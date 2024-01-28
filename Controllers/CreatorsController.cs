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

            if (image != null && image.Length > 0)
            {
                string ext = Path.GetExtension(image.FileName);
                string fileName = creator.Id + ext;
                using (Stream fileStream = new FileStream("/appdata/img/" + fileName, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }
                creator.ImagePath = "/img/" + fileName;
            }

            _context.Add(creator);
            await _context.SaveChangesAsync();

            return Json(creator);
        }

        // GET: Creators/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Creator == null)
            {
                return NotFound();
            }

            var creator = await _context.Creator.FindAsync(id);
            if (creator == null)
            {
                return NotFound();
            }
            return View(creator);
        }

        // POST: Creators/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Icon")] Creator creator)
        {
            if (id != creator.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(creator);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CreatorExists(creator.Id))
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
            return View(creator);
        }

        // GET: Creators/Delete/5
        public async Task<IActionResult> Delete(string id)
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

        // POST: Creators/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
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
            return RedirectToAction(nameof(Index));
        }

        private bool CreatorExists(string id)
        {
          return (_context.Creator?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
