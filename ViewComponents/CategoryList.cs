using HamBurger.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace HamBurger.ViewComponents
{
    public class CategoryList : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public CategoryList(ApplicationDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var category = _context.Categories.ToList();
            return View(category);
        }
    }
}
