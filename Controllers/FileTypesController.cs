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
    public class FileTypesController : Controller
    {
        private readonly DatabaseContext _context;

        public FileTypesController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: FileTypes
        public async Task<IActionResult> Index()
        {
              return _context.FileType != null ? 
                          View(await _context.FileType.ToListAsync()) :
                          Problem("Entity set 'DatabaseContext.FileType'  is null.");
        }

        // GET: FileTypes/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.FileType == null)
            {
                return NotFound();
            }

            var fileType = await _context.FileType
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fileType == null)
            {
                return NotFound();
            }

            return View(fileType);
        }

        // GET: FileTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FileTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Extensions,Name,Icon")] FileType fileType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fileType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fileType);
        }

        // GET: FileTypes/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.FileType == null)
            {
                return NotFound();
            }

            var fileType = await _context.FileType.FindAsync(id);
            if (fileType == null)
            {
                return NotFound();
            }
            return View(fileType);
        }

        // POST: FileTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Extensions,Name,Icon")] FileType fileType)
        {
            if (id != fileType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fileType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FileTypeExists(fileType.Id))
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
            return View(fileType);
        }

        // GET: FileTypes/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.FileType == null)
            {
                return NotFound();
            }

            var fileType = await _context.FileType
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fileType == null)
            {
                return NotFound();
            }

            return View(fileType);
        }

        // POST: FileTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.FileType == null)
            {
                return Problem("Entity set 'DatabaseContext.FileType'  is null.");
            }
            var fileType = await _context.FileType.FindAsync(id);
            if (fileType != null)
            {
                _context.FileType.Remove(fileType);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FileTypeExists(string id)
        {
          return (_context.FileType?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
