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
                Id = IdGen.GetBase36(8);
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

        // POST: Categories/Edit/5
        [HttpPost]
        public async Task<JsonResult> Edit(string id, string name, IFormFile image)
        {
            var category = _context.Category.Where(w => w.Id == id).FirstOrDefault();
            if (category == null)
            {
                return new JsonResult(NotFound());
            }
            category.Name = name;

            var imgPath = await SaveImage(image, category);
            if (!string.IsNullOrEmpty(imgPath))
            {
                category.ImagePath = imgPath;
            }

            _context.Update(category);
            await _context.SaveChangesAsync();

            return Json(category);
        }

        private async Task<string> SaveImage(IFormFile image, Category category)
        {
            if (image != null && image.Length > 0)
            {
                string ext = Path.GetExtension(image.FileName);
                string fileName = category.Id + ext;
                using (Stream fileStream = new FileStream("/appdata/img/" + fileName, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }
                return "/img/" + fileName;
            }
            return "";
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

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id, string moveToId)
        {
            var category = await _context.Category.FindAsync(id);
            if (category == null)
            {
                return Problem("category is null.");
            }

            //get all of the prints in the Category we are going to delete and move them to the new one.
            var prints = _context.Print.Where(w => w.CategoryId != null && w.CategoryId == id);
            foreach( var print in prints ){
                print.CategoryId = moveToId;
            }

            //delete thumbnail image
            if (!string.IsNullOrEmpty(category.ImagePath))
            {
                System.IO.File.Delete("/appdata/" + category.ImagePath);
            }
            
            //remove the category and save changes
            _context.Category.Remove(category);
            await _context.SaveChangesAsync();
            

            return RedirectToAction("Settings", "Home");
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
