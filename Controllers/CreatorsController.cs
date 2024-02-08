using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PrintBed.Models;

namespace PrintBed.Controllers
{
    public class CreatorsController : Controller
    {
        private readonly DatabaseContext _context;

        public CreatorsController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: Creators
        public async Task<IActionResult> Index()
        {
              return _context.Creator != null ? 
                          View(await _context.Creator.ToListAsync()) :
                          Problem("Entity set 'DatabaseContext.Creator'  is null.");
        }

        // GET: Creators/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Creator == null)
            {
                return NotFound();
            }

            var creator = await _context.Creator
                .FirstOrDefaultAsync(m => m.Id == id);
            if (creator == null)
            {
                return NotFound();
            }

            return View(creator);
        }
        
        [HttpPost]
        public async Task<JsonResult> Create(string name, IFormFile image)
        {
            var creator = new Creator();
            creator.Name = name;
            creator.Id = IDGen.GetBase62(6);

            creator.ImagePath = await SaveImage(image, creator);

            _context.Add(creator);
            await _context.SaveChangesAsync();

            return Json(creator);
        }

        // POST: Creators/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<JsonResult> Edit(string id, string name, IFormFile image)
        {
            var creator = _context.Creator.Where(w => w.Id == id).FirstOrDefault();
            if(creator==null)
            {
                return new JsonResult(NotFound());
            }
            creator.Name = name;

            var imgPath = await SaveImage(image, creator);
            if (!string.IsNullOrEmpty(imgPath))
            {
                creator.ImagePath = imgPath;
            }

            _context.Update(creator);
            await _context.SaveChangesAsync();

            return Json(creator);
        }
             
        private async Task<string> SaveImage(IFormFile image, Creator creator)
        {
            if (image != null && image.Length > 0)
            {
                string ext = Path.GetExtension(image.FileName);
                string fileName = creator.Id + ext;
                using (Stream fileStream = new FileStream("/appdata/img/" + fileName, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }
                return "/img/" + fileName;
            }
            return "";
        }

        // POST: Creators/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Creator == null)
            {
                return Problem("Entity set 'DatabaseContext.Creator'  is null.");
            }
            var creator = await _context.Creator.FindAsync(id);
            if (creator != null)
            {
                _context.Creator.Remove(creator);
            }
            
            await _context.SaveChangesAsync();
            return Ok();
        }

        private bool CreatorExists(string id)
        {
          return (_context.Creator?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
