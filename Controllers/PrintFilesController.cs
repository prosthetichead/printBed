using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
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

        private string SafeString(string str)
        {
            str = str.Trim();
            str = str.ToLower();
            str = str.Replace(' ', '-');
            
            List<char> invalidFileNameChars = Path.GetInvalidFileNameChars().ToList<char>();
            invalidFileNameChars.AddRange("!*'();:@&=+$,/?%#[]");
            // Builds a string out of valid chars and an _ for invalid ones
            var safeStr = new string(str.Select(ch => invalidFileNameChars.Contains(ch) ? '_' : ch).ToArray());
            return safeStr;
        }

        // POST: PrintFiles/FileUpload
        [HttpPost]
        public async Task<IActionResult> FileUpload(IFormFile uploadedFile, string printId)
        {

            //get the print details and work out where we need to put this file
            Print? print = _context.Print.Where(w=>w.Id == printId).Include(p=>p.Creator).Include(p=>p.Category).FirstOrDefault();
            if (print == null)
            {
                return NotFound();
            }

            //setup and save a print file record for the file
            PrintFile printFile = new PrintFile();
            printFile.Id = IDGen.GetBase62(6);
            printFile.PrintId = printId;
            printFile.DisplayName = uploadedFile.FileName;
            printFile.FileName = SafeString(uploadedFile.FileName);
            printFile.FileTypeId = "0";
            printFile.FileSize = uploadedFile.Length;

            string filePath = Path.Combine("/print-files", SafeString(print.Category.Name), SafeString(print.Creator.Name), SafeString(print.Name));
            Directory.CreateDirectory(filePath);
            filePath = Path.Combine(filePath, printFile.FileName);
            printFile.FilePath = filePath;
            string ext = Path.GetExtension(filePath).Replace(".", "");
            printFile.FileExtension = ext;

            var fileType = _context.FileType.Where(w => w.Extensions != null && w.Extensions.Contains(ext)).FirstOrDefault();
            if (fileType != null)
            {
                printFile.FileTypeId = fileType.Id;
            }

            using (Stream fileStream = new FileStream(filePath, FileMode.Create))
            {
                await uploadedFile.CopyToAsync(fileStream);
            }

            _context.PrintFile.Add(printFile);
            _context.SaveChanges();

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
                .Include(p => p.Print)
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
