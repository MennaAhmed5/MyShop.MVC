using Microsoft.AspNetCore.Mvc;
using MyShop.Web.Data;
using MyShop.Web.Models;

namespace MyShop.Web.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var categories = _context.Categories.ToList();

            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Add(category);
                _context.SaveChanges();
                TempData["Create"] = "Data Has Created Successfully";
                return RedirectToAction("index");
            }
            return View(category);
        }

        public IActionResult Edit(int? id)
        {
            if(id==null || id == 0)
            {
                NotFound();
            }
            var CategoryinDb = _context.Categories.Find(id);
            return View(CategoryinDb);
        }
        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Update(category);
                _context.SaveChanges();
                TempData["Update"] = "Data Has Updated Successfully";

                return RedirectToAction("index");
            }
            return View(category);
        }
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                NotFound();
            }
            var CategoryinDb = _context.Categories.Find(id);
            return View(CategoryinDb);
        }
        [HttpPost]
        public IActionResult DeleteCategory(int? id)
        {
            var CategoryInDb = _context.Categories.Find(id);

            if (CategoryInDb == null)
            {
                NotFound();
            }
            _context.Categories.Remove(CategoryInDb);
            _context.SaveChanges();
            TempData["Delete"] = "Data Has Deleted Successfully";
            return RedirectToAction("Index");
        }
    }
}
