﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PrintBed.Models;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Linq.Dynamic.Core;

namespace PrintBed.Controllers
{
    public class HomeController : Controller
    {
        private readonly DatabaseContext _context;
        //private readonly ILogger<HomeController> _logger;

        public HomeController(DatabaseContext context)
        {
            //_logger = logger;
            _context = context;
        }


        public async Task<IActionResult> Settings()
        {
            var settingsPage = new SettingsPage();
            var categories = _context.Category.Include(i=>i.Prints);
            var creators = _context.Creator.Include(i => i.Prints);

            settingsPage.Categories = await categories.ToListAsync();
            settingsPage.Creators = await creators.ToListAsync();

            return View(settingsPage);
        }

        public async Task<IActionResult> Index(
            [FromQuery(Name = "creators")] List<string> creators,
            [FromQuery(Name = "page")] int page = 1,
            [FromQuery(Name = "search")] string search = "",            
            [FromQuery(Name = "sort")] string sort = "Name", 
            [FromQuery(Name = "direction")] string direction = "DESC")
        {

            int itemsPerPage = 12;
            int totalPages = 0;

            var prints = _context.Print.Include(m => m.Category).Include(m => m.Creator).Include(m=>m.PrintFiles).ThenInclude(m=>m.FileType)
                .Where(w => 
                    ((creators.Count()>0 && creators.Contains(w.CreatorId)) || creators.Count() == 0)  &&
                    ((!string.IsNullOrEmpty(search) && (w.Name.ToLower().Contains(search.ToLower()) || w.TagString.ToLower().Contains(search.ToLower()))) || string.IsNullOrEmpty(search))
                );
            prints = prints.OrderBy(sort + " " + direction);

            totalPages = (int)Math.Ceiling((double)prints.Count() / itemsPerPage);
            page = page - 1;
            page = page <= 0 || page >= totalPages ? 0 : page; //limit pages to our total pages and make page 1 = page 0

            prints = prints.Skip(page * itemsPerPage).Take(itemsPerPage);

            ViewData["CurrentPage"] = page+1;
            ViewData["TotalPages"] = totalPages;

            MultiSelectList Categories = new MultiSelectList(_context.Category, "Id", "Name");
            ViewData["Categories"] = Categories;

            MultiSelectList Creators = new MultiSelectList(_context.Creator, "Id", "Name", creators);
            ViewData["Creators"] = Creators;

            SelectList Tags = new SelectList(_context.Tag, "Id", "Name");
            ViewData["Tags"] = Tags;

            SelectList SortList = new SelectList(new List<SelectListItem>() {
                new SelectListItem("Name", "Name"),
                new SelectListItem("Created Date", "Created"),
            }, "Value", "Text", sort);
            SelectList DirectionList = new SelectList(new List<SelectListItem>() {
                new SelectListItem("Descending", "DESC"),
                new SelectListItem("Ascending", "ASC"),
            }, "Value", "Text", direction);

            ViewData["DirectionList"] = DirectionList;
            ViewData["direction"] = direction;
            ViewData["SortList"] = SortList;
            ViewData["sort"] = sort;
            return View(await prints.ToListAsync());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}