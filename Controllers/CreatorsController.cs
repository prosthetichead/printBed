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

            var Id = "new";
            while(Id == "new")
            {
                Id = IDGen.GetBase36(8);
                if(_context.Creator.Any(m => m.Id == Id))
                {
                    Id = "new";
                }
            }
            creator.Id = Id;

            creator.ImagePath = await SaveImage(image, creator);

            _context.Add(creator);
            await _context.SaveChangesAsync();

            return Json(creator);
        }

        // POST: Creators/Edit/5
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


        public async Task<IActionResult> Delete(string id)
        {
            var creator = await _context.Creator
                .Include(p => p.Prints)
                .FirstOrDefaultAsync(m => m.Id == id);

            ViewData["creatorId"] = new SelectList(_context.Creator.Where(w => w.Id != id).OrderBy(o => o.Name), "Id", "Name", "0");

            if (creator == null)
            {
                return NotFound();
            }

            return View(creator);
        }





        [HttpPost]
        public async Task<JsonResult> Delete(string id, string newId = "0")
        {
            try
            {
                var creator = await _context.Creator.Where(w=>w.Id == id).Include(i=>i.Prints).FirstAsync();
                if (creator != null)
                {
                    foreach(var print  in creator.Prints)
                    {
                        print.CreatorId = newId; //set creator to the new id provided.
                    }
                    
                    _context.Creator.Remove(creator);
                    
                    await _context.SaveChangesAsync();

                }
            }
            catch (Exception ex)
            {
            }   


            return Json(Ok());
        }

    }
}
