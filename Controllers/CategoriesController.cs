using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PrintBed.Helpers;
using PrintBed.Models;

namespace PrintBed.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly DatabaseContext _context;

        public CategoriesController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
              return _context.Category != null ? 
                          View(await _context.Category.ToListAsync()) :
                          Problem("Entity set 'DatabaseContext.Category'  is null.");
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Category == null)
            {
                return NotFound();
            }

            var category = await _context.Category
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        public async Task<JsonResult> Create(string name, IFormFile image)
        {
            var category = new Category();
            category.Name = name;
            var Id = "new";
            while(Id == "new")
            {
                Id = IDGen.GetBase36(8);
                if(_context.Category.Any(m => m.Id == Id))
                {
                    Id = "new";
                }
            }
            category.Id = Id;


            _context.Add(category);
            await _context.SaveChangesAsync();

            return Json(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Category == null)
            {
                return NotFound();
            }

            var category = await _context.Category.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Icon")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
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
            return View(category);
        }


        public async Task<IActionResult> Delete(string id)
        {
            var category = await _context.Category
                .Include(p => p.Prints)
                .FirstOrDefaultAsync(m => m.Id == id);

            ViewData["CategoryId"] = new SelectList(_context.Category.Where(w=>w.Id != id).OrderBy(o => o.Name), "Id", "Name", "0");

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: PrintFiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id, string moveToId)
        {
            if (_context.PrintFile == null)
            {
                return Problem("Entity set 'DatabaseContext.PrintFiles'  is null.");
            }
            var printFile = await _context.PrintFile.FindAsync(id);
            if (printFile == null)
            {
                return Problem("printFile is null.");
            }

            _context.PrintFile.Remove(printFile);
            await _context.SaveChangesAsync();

            System.IO.File.Delete(printFile.FilePath);

            return RedirectToAction("Details", "Prints", new { id = printFile.PrintId });
        }


        [HttpPost]
        public async Task<JsonResult> QuickDelete(string id)
        {
            var category = await _context.Category.FindAsync(id);
            if (category != null)
            {
                _context.Category.Remove(category);
                await _context.SaveChangesAsync();
            }

            return Json(Ok());
        }

        private bool CategoryExists(string id)
        {
          return (_context.Category?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
