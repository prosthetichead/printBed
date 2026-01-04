using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PrintBed.Models;

namespace PrintBed.Controllers
{
    public class TagsController : Controller
    {
        private readonly DatabaseContext _context;

        public TagsController(DatabaseContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<JsonResult> Create(string name)
        {
            var id = Helpers.SafeString.Convert(name);
            var tag = new Tag();
            //check if ID already is in our list
            if (_context.Tag.Any(w=>w.Id == id)){
                //get the row and return the one we already have
                tag = _context.Tag.Where(w => w.Id == id).FirstOrDefault(); 
                return Json(tag);
            }

            tag = new Tag { Name = name, Id = id };
            _context.Add(tag);
            await _context.SaveChangesAsync();

            return Json(tag);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var tag = await _context.Tag
                .Include(p => p.PrintTags)
                .FirstOrDefaultAsync(m => m.Id == id);

            ViewData["TagId"] = new SelectList(_context.Tag.Where(w => w.Id != id).OrderBy(o => o.Name), "Id", "Name", "0");

            if (tag == null)
            {
                return NotFound();
            }

            return View(tag);
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
            foreach (var print in prints)
            {
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

    }
}
