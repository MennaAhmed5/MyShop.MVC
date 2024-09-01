using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using MyShop.DataAccess;
using MyShop.Entities.Models;
using MyShop.Entities.Repositories;
using MyShop.Entities.ViewModels;


namespace MyShop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork , IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {

 
            return View();
        }

        public IActionResult GetData()
        {
            var products = _unitOfWork.Product.GetAll(includeword:"Category");
            return Json(new { data = products });
        }

 
        

        [HttpGet]
        public IActionResult Create()
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategoryList = _unitOfWork.Category.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })

            };
            
            return View(productVM);
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductVM productVM, IFormFile file)
        {

            if (ModelState.IsValid)
            {
                string rootPath = _webHostEnvironment.WebRootPath; //wwwroot folder
                if(file != null)
                {
                    string filename = Guid.NewGuid().ToString();
                    var Upload = Path.Combine(rootPath, @"Images\Products");
                    var ext = Path.GetExtension(file.FileName);

                    using(var fileStream = new FileStream(Path.Combine(Upload, filename + ext), FileMode.Create))
                    {
                        file.CopyTo(fileStream);       
                    }
                    productVM.Product.Img = @"Images\Products\" + filename + ext;
                }
                //_context.Categories.Add(Product);
                _unitOfWork.Product.Add(productVM.Product);
                //_context.SaveChanges();
                _unitOfWork.complete();
                TempData["Create"] = "Data Has Created Successfully";
                return RedirectToAction("index");
            }
            return View(productVM.Product);
        }
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                NotFound();
            }
            // var ProductinDb = _context.Categories.Find(id);
            var ProductinDb = _unitOfWork.Product.GetFirstOrDefault(x => x.Id == id);
            ProductVM productVM = new ProductVM()
            {
                Product = _unitOfWork.Product.GetFirstOrDefault(x => x.Id == id),
                CategoryList = _unitOfWork.Category.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };
            return View(productVM);
        }
        [HttpPost]
        public IActionResult Edit(ProductVM productVM , IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string rootPath = _webHostEnvironment.WebRootPath; //wwwroot folder
                if (file != null)
                {
                    string filename = Guid.NewGuid().ToString();
                    var Upload = Path.Combine(rootPath, @"Images\Products");
                    var ext = Path.GetExtension(file.FileName);

                    if(productVM.Product.Img != null)
                    {
                        var olding = Path.Combine(rootPath, productVM.Product.Img.TrimStart('\\'));
                        if(System.IO.File.Exists(olding))
                        {
                            System.IO.File.Delete(olding);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(Upload, filename + ext), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVM.Product.Img = @"Images\Products\" + filename + ext;
                }
                _unitOfWork.Product.Update(productVM.Product);
                _unitOfWork.complete();
                TempData["Update"] = "Data Has Updated Successfully";

                return RedirectToAction("Index");
            }
            return View(productVM.Product);
        }
        
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var ProductInDb = _unitOfWork.Product.GetFirstOrDefault(x => x.Id == id);

            if (ProductInDb == null)
            {
                Json(new { success = false, message = "Error While Deleting" });

            }
            _unitOfWork.Product.Remove(ProductInDb);
            var olding = Path.Combine(_webHostEnvironment.WebRootPath, ProductInDb.Img.TrimStart('\\'));
            if (System.IO.File.Exists(olding))
            {
                System.IO.File.Delete(olding);
            }
            _unitOfWork.complete();
            return Json(new { success = true, message = "Product has been Deleted" });

         }
    }
}
