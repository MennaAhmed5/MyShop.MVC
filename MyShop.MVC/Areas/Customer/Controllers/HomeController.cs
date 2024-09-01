using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShop.Entities.Models;
using MyShop.Entities.Repositories;
using System.Security.Claims;

namespace MyShop.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var products = _unitOfWork.Product.GetAll();
            return View(products);
        }
        [HttpGet]
        public IActionResult Details(int productId)
        {
            ShoppingCart obj = new ShoppingCart()
            {
                ProductId = productId,
                Product = _unitOfWork.Product.GetFirstOrDefault(x => x.Id == productId, includeword: "Category"),
                Count = 1,
                
            };
             return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCart.ApplicationUserId = claim.Value;
            _unitOfWork.ShoppingCart.Add(shoppingCart);
            _unitOfWork.complete();
            return RedirectToAction("Index"); 
        }
    }
}
