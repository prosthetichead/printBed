using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PrintBed.Helpers;
using PrintBed.Models;

namespace PrintBed.Controllers
{
    public class PrintsController : Controller
    {
        private readonly DatabaseContext _context;

        public PrintsController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: Prints/Details/5
        public async Task<IActionResult> Details(string id, [FromQuery(Name = "page")] int page = 1)
        {            
            int itemsPerPage = 8;
            int totalPages = 0;
            
            PrintDetailPage printDetailPage = new PrintDetailPage();

            var print = await _context.Print.Include( m=>m.PrintFiles.OrderBy(o=>o.FileExtension) ).ThenInclude(m=>m.FileType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (print == null)
            {
                return NotFound();
            }
            totalPages = (int)Math.Ceiling((double)print.PrintFiles.Count() / itemsPerPage);
            page = page - 1;
            page = page <= 0 || page >= totalPages ? 0 : page; //limit pages to our total pages and make page 1 = page 0
            printDetailPage.Print = print;
            printDetailPage.Files = print.PrintFiles.Skip(page*itemsPerPage).Take(itemsPerPage).ToList();

            printDetailPage.totalPages = totalPages;
            printDetailPage.currentPage = page+1;

            SelectList FileTypesList = new SelectList(_context.FileType, "Id", "Name", "0");      
            
            ViewData["FileTypesList"] = FileTypesList;
            ViewData["Referer"] = Request.Headers["Referer"];

            return View(printDetailPage);
        }

        // GET: Prints/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Category.OrderBy(o => o.Name), "Id", "Name");
            ViewData["CreatorId"] = new SelectList(_context.Creator.OrderBy(o => o.Name), "Id", "Name");
            ViewData["Tags"] = new SelectList(_context.Tag.OrderBy(o=>o.Name), "Id", "Name");
            return View();
        }

        // POST: Prints/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,PrintInstructions,CreatorId,CategoryId")] Print print, List<string> tags)
        {
            string Id = "new";
            while (Id == "new")
            {
                Id = IDGen.GetBase36(8);
                if(_context.Print.Any(o => o.Id == Id))
                {
                    Id = "new";
                }                
            }
            print.Id = Id;
            print.TagString = string.Join(",",tags);

            //create printtags
            foreach(var tag in tags)
            {
                PrintTag printTag = new PrintTag() { Id = Id+"#"+tag, PrintId = Id, TagId = tag};
                _context.Add(printTag);
            }

            if (ModelState.IsValid)
            {
                _context.Add(print);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = print.Id });
            }

            return View(print);
        }

        // GET: Prints/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Print == null)
            {
                return NotFound();
            }

            var print = await _context.Print.FindAsync(id);
            if (print == null)
            {
                return NotFound();
            }
            return View(print);
        }

        // POST: Prints/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Description,PrintInstructions")] Print print)
        {
            if (id != print.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(print);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PrintExists(print.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(print);
        }

        // GET: Prints/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Print == null)
            {
                return NotFound();
            }

            var print = await _context.Print.Include(i=>i.PrintFiles)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (print == null)
            {
                return NotFound();
            }

            ViewData["Referer"] = Request.Headers["Referer"];
            return View(print);
        }

        // POST: Prints/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Print == null)
            {
                return Problem("Entity set 'DatabaseContext.Prints'  is null.");
            }
            var print = await _context.Print.FindAsync(id);
            
            if (print != null)
            {
                var printFiles = _context.PrintFile.Where(w=>w.PrintId == id);
                foreach (var file in printFiles)
                {
                    System.IO.File.Delete(file.FilePath);
                    _context.PrintFile.Remove(file);
                }
                _context.Print.Remove(print);
                
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction("Index","Home");
        }

        private bool PrintExists(string id)
        {
          return (_context.Print?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
