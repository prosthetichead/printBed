using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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



        // POST: PrintFiles/FileUpload
        [HttpPost]
        public async Task<IActionResult> FileUpload(List<IFormFile> uploadedFiles, string printId)
        {
            if (uploadedFiles != null && uploadedFiles.Count > 0 && !string.IsNullOrEmpty(printId))
            {
                //get the print details and work out where we need to put this file
                Print? print = _context.Print.Where(w => w.Id == printId).Include(p => p.Creator).Include(p => p.Category).FirstOrDefault();
                if (print == null)
                {
                    return NotFound();
                }

                //Define the directory path (This is constant for all files in this batch)
                string baseUploadPath = Path.Combine("/print-files",
                    Helpers.SafeString.Convert(print.Category.Name),
                    Helpers.SafeString.Convert(print.Creator.Name),
                    Helpers.SafeString.Convert(print.Name));

                Directory.CreateDirectory(baseUploadPath);

                //Loop through every file uploaded
                foreach (var file in uploadedFiles)
                {
                    // Skip empty files if any
                    if (file.Length == 0) continue;

                    // Setup and save a print file record for the file
                    PrintFile printFile = new PrintFile();

                    var Id = "new";
                    while (Id == "new")
                    {
                        Id = IdGen.GetBase36(8);
                        if (_context.PrintFile.Any(w => w.Id == Id))
                        {
                            Id = "new";
                        }
                    }

                    printFile.Id = Id;
                    printFile.PrintId = printId;
                    
                    string safeFileName = Helpers.SafeString.Convert(file.FileName);
                    string fullFilePath = Path.Combine(baseUploadPath, safeFileName);
                    //Check if file exists, and loop until we find a unique name
                    string finalFileName = safeFileName;
                    if (System.IO.File.Exists(fullFilePath))
                    {
                        int counter = 1;
                        string fileNameOnly = Path.GetFileNameWithoutExtension(safeFileName);
                        string extension = Path.GetExtension(safeFileName);

                        while (System.IO.File.Exists(fullFilePath))
                        {
                            // Create a new name like: "myFile_1.stl"
                            finalFileName = $"{fileNameOnly}_{counter}{extension}";
                            fullFilePath = Path.Combine(baseUploadPath, finalFileName);
                            counter++;
                        }
                    }
                    printFile.FileName = finalFileName;
                    printFile.DisplayName = finalFileName; 
                    printFile.FilePath = fullFilePath;
                    printFile.FileSize = file.Length;
                    printFile.FilePath = fullFilePath;

                    string ext = Path.GetExtension(fullFilePath).Replace(".", "");
                    printFile.FileExtension = ext;

                    // File Type Logic
                    var fileType = _context.FileType
                        .Where(w => w.Extensions != null && w.Extensions.ToLower().Contains(ext))
                        .FirstOrDefault();
                    if (fileType != null)
                    {
                        printFile.FileTypeId = fileType.Id;
                    }
                    else
                    {
                        var defaultType = _context.FileType.FirstOrDefault(w => w.Id == "0");
                        printFile.FileTypeId = defaultType != null ? defaultType.Id : "0";
                    }
                    
                    using (Stream fileStream = new FileStream(fullFilePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    _context.PrintFile.Add(printFile);
                }

                // Save all database changes
                await _context.SaveChangesAsync();

                return Ok();
            }

            return Ok();
        }


        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.PrintFile == null)
            {
                return NotFound();
            }

            var printFile = await _context.PrintFile.Where(w => w.Id == id).FirstOrDefaultAsync();

            if (printFile == null)
            {
                return NotFound();
            }

            ViewData["Referer"] = Request.Headers["Referer"];

            
            return View(printFile);
        }

        // POST: PrintFiles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,DisplayName,Description,IsPreview")] PrintFile printFile, string referer)
        {
            if (id != printFile.Id)
            {
                return NotFound();
            }
            //get the existing record
            var existingPrintFile = await _context.PrintFile.Where(w => w.Id == id).FirstOrDefaultAsync();
            if (existingPrintFile == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    existingPrintFile.DisplayName = printFile.DisplayName;
                    existingPrintFile.Description = printFile.Description;
                    existingPrintFile.IsPreview = printFile.IsPreview;
                    if(printFile.IsPreview)
                    {
                        //set all other files for this print to not be preview
                        var otherFiles = _context.PrintFile.Where(w => w.PrintId == existingPrintFile.PrintId && w.Id != existingPrintFile.Id).ToList();
                        foreach(var otherFile in otherFiles)
                        {
                            otherFile.IsPreview = false;
                            _context.Update(otherFile);
                        }
                    }

                    existingPrintFile.LastModified = DateTime.Now;
                    _context.Update(existingPrintFile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PrintFileExists(printFile.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Prints", new { id = existingPrintFile.PrintId });
            }
            
            return NotFound();

        }


        public async Task<IActionResult> Download(string id)
        {
            var printFile = await _context.PrintFile
                .Include(p => p.Print).Include(p => p.FileType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (printFile == null)
            {
                return NotFound();
            }

            return Redirect(printFile.FilePath);
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
            if (printFile == null)
            {
                return Problem("printFile is null.");
            }

            _context.PrintFile.Remove(printFile);
            await _context.SaveChangesAsync();

            System.IO.File.Delete(printFile.FilePath);
            
            return RedirectToAction("Details", "Prints", new { id = printFile.PrintId });
        }

        private bool PrintFileExists(string id)
        {
          return (_context.PrintFile?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
