using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PrintBed.Helpers;
using PrintBed.Models;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

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

            var print = await _context.Print.Include( m=>m.PrintFiles.OrderBy(o=>o.FileExtension) ).ThenInclude(m=>m.FileType).Include(m=>m.PrintTags).ThenInclude(m=>m.Tag)
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


            if (ModelState.IsValid)
            {
                _context.Add(print);

                await _context.SaveChangesAsync();
                UpsertTags(Id, tags);
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

            var print = await _context.Print.Include(i=>i.PrintTags).Where(w=>w.Id == id).FirstOrDefaultAsync();
            
            if (print == null)
            {
                return NotFound();
            }

            ViewData["Referer"] = Request.Headers["Referer"];

            ViewData["CategoryId"] = new SelectList(_context.Category.OrderBy(o => o.Name), "Id", "Name", print.CategoryId);
            ViewData["CreatorId"] = new SelectList(_context.Creator.OrderBy(o => o.Name), "Id", "Name", print.CreatorId); ;
            ViewData["Tags"] = new MultiSelectList(_context.Tag.OrderBy(o => o.Name), "Id", "Name", print.PrintTags.Select(s=>s.TagId));

            return View(print);
        }

        // POST: Prints/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Description,PrintInstructions,CreatorId,CategoryId")] Print print, List<string> tags, string referer)
        {
            if (id != print.Id)
            {
                return NotFound();
            }

            

            //
            print.TagString = string.Join(",", tags);
            var printTags = UpsertTags(id, tags);

            _context.Print.Update(print);
            _context.SaveChanges();

            return Redirect(referer);
        }
        
        public List<PrintTag> UpsertTags(string printId, List<string> tags)
        {
            //remove all current tags
            var printTags = _context.PrintTag.Where(w => w.PrintId == printId);
            _context.RemoveRange(printTags);

            var uniqueTagsMap = new Dictionary<string, string>();
            foreach (var tag in tags)
            {
                var safeId = Helpers.SafeString.Convert(tag);

                // Only add if we haven't processed this ID yet
                if (!string.IsNullOrEmpty(safeId) && !uniqueTagsMap.ContainsKey(safeId))
                {
                    uniqueTagsMap.Add(safeId, tag);
                }
            }

            foreach (var item in uniqueTagsMap)
            {
                var tagId = item.Key;   
                var tagName = item.Value; 

                //check if tag exists
                var existingTag = _context.Tag.Find(tagId);
                if (existingTag == null)
                {
                    // 3. Create new tag and assign it directly to the variable
                    existingTag = new Tag() { Id = tagId, Name = tagName };
                    _context.Add(existingTag);
                    _context.SaveChanges();
                }

                //create new print tag
                PrintTag printTag = new PrintTag()
                {
                    Id = printId + "#" + tagId,
                    PrintId = printId,
                    Tag = existingTag
                };
                _context.Add(printTag);
            }

            _context.SaveChanges();

            printTags = _context.PrintTag.Where(w => w.PrintId == printId);
            return printTags.ToList();
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
