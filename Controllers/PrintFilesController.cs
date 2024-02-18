using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using PrintBed.Helpers;
using PrintBed.Models;

namespace PrintBed.Controllers
{
    public class PrintFilesController : Controller
    {
        private readonly DatabaseContext _context;

        public PrintFilesController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: PrintFiles
        public async Task<IActionResult> Index()
        {
            var data = _context.PrintFile.Include(p => p.Print);

            return View(await data.ToListAsync());
        }

        // GET: PrintFiles/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.PrintFile == null)
            {
                return NotFound();
            }

            var printFile = await _context.PrintFile
                .Include(p => p.Print)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (printFile == null)
            {
                return NotFound();
            }

            return View(printFile);
        }

        //// GET: PrintFiles/Create
        //public IActionResult Create()
        //{
        //    ViewData["PrintId"] = new SelectList(_context.Print, "Id", "Id");
        //    return View();
        //}

        //// POST: PrintFiles/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,Name,Description,Type,PrintId")] PrintFile printFile)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(printFile);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["PrintId"] = new SelectList(_context.Print, "Id", "Id", printFile.PrintId);
        //    return View(printFile);
        //}



        // POST: PrintFiles/FileUpload
        [HttpPost]
        public async Task<IActionResult> FileUpload(IFormFile uploadedFile, string printId)
        {
            if (uploadedFile != null)
            {
                //get the print details and work out where we need to put this file
                Print? print = _context.Print.Where(w => w.Id == printId).Include(p => p.Creator).Include(p => p.Category).FirstOrDefault();
                if (print == null)
                {
                    return NotFound();
                }

                //setup and save a print file record for the file
                PrintFile printFile = new PrintFile();

                var Id = "new";
                while (Id == "new")
                {
                    Id = IDGen.GetBase36(8);
                    if (_context.PrintFile.Any(w => w.Id == Id))
                    {
                        Id = "new";
                    }
                }

                printFile.Id = Id;
                printFile.PrintId = printId;
                printFile.DisplayName = uploadedFile.FileName;
                printFile.FileName = Helpers.SafeString.Convert(uploadedFile.FileName);
                printFile.FileTypeId = "0";
                printFile.FileSize = uploadedFile.Length;

                string filePath = Path.Combine("/print-files", Helpers.SafeString.Convert(print.Category.Name), Helpers.SafeString.Convert(print.Creator.Name), Helpers.SafeString.Convert(print.Name));
                Directory.CreateDirectory(filePath);
                filePath = Path.Combine(filePath, printFile.FileName);
                printFile.FilePath = filePath;
                string ext = Path.GetExtension(filePath).Replace(".", "");
                printFile.FileExtension = ext;

                var fileType = _context.FileType.Where(w => w.Extensions != null && w.Extensions.ToLower().Contains(ext)).FirstOrDefault();
                if (fileType != null)
                {
                    printFile.FileTypeId = fileType.Id;
                }
                else
                {
                    printFile.FileTypeId = _context.FileType.Where(w => w.Id == "0").FirstOrDefault().Id;
                }

                using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }

                _context.PrintFile.Add(printFile);
                _context.SaveChanges();

                return Ok();
            }
            return Ok();
        }

        // POST: PrintFiles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditName(string id, string name)
        {
            
            return Ok();
        }

        // GET: PrintFiles/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.PrintFile == null)
            {
                return NotFound();
            }

            var printFile = await _context.PrintFile
                .Include(p => p.Print).Include(p=>p.FileType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (printFile == null)
            {
                return NotFound();
            }

            return View(printFile);
        }

        // POST: PrintFiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.PrintFile == null)
            {
                return Problem("Entity set 'DatabaseContext.PrintFiles'  is null.");
            }
            var printFile = await _context.PrintFile.FindAsync(id);
            if (printFile != null)
            {
                _context.PrintFile.Remove(printFile);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PrintFileExists(string id)
        {
          return (_context.PrintFile?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
