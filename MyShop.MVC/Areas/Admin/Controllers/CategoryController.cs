using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyShop.DataAccess;
using MyShop.Entities.Models;
using MyShop.Entities.Repositories;


namespace MyShop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var categories = _unitOfWork.Category.GetAll();

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
                //_context.Categories.Add(category);
                _unitOfWork.Category.Add(category);
                //_context.SaveChanges();
                _unitOfWork.complete();
                TempData["Create"] = "Data Has Created Successfully";
                return RedirectToAction("index");
            }
            return View(category);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                NotFound();
            }
            // var CategoryinDb = _context.Categories.Find(id);
            var CategoryinDb = _unitOfWork.Category.GetFirstOrDefault(x => x.Id == id);
            return View(CategoryinDb);
        }
        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                // _context.Categories.Update(category);
                //_context.SaveChanges();
                _unitOfWork.Category.Update(category);
                _unitOfWork.complete();
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
            var CategoryinDb = _unitOfWork.Category.GetFirstOrDefault(x => x.Id == id);
            return View(CategoryinDb);
        }
        [HttpPost]
        public IActionResult DeleteCategory(int? id)
        {
            var CategoryInDb = _unitOfWork.Category.GetFirstOrDefault(x => x.Id == id);

            if (CategoryInDb == null)
            {
                NotFound();
            }
            _unitOfWork.Category.Remove(CategoryInDb);
            _unitOfWork.complete();
            TempData["Delete"] = "Data Has Deleted Successfully";
            return RedirectToAction("Index");
        }
    }
}
