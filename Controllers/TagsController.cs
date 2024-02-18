using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost]
        public async Task<JsonResult> Delete(string id)
        {
            var tag = await _context.Tag.FindAsync(id);
            if (tag != null)
            {
                var printtags = _context.PrintTag.Where(w => w.TagId == id);
                _context.PrintTag.RemoveRange(printtags);
                _context.Tag.Remove(tag);
                await _context.SaveChangesAsync();
            }

            return Json(Ok());  
        }
    }
}
