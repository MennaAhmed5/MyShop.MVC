using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShop.Entities.Models;
using MyShop.Entities.Repositories;
using MyShop.Utilities;
using System.Security.Claims;
using X.PagedList;


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
        public IActionResult Index(int? page)
        {
            var PageNumber = page ?? 1;
            int PageSize = 8;

            var products = _unitOfWork.Product.GetAll().ToPagedList(PageNumber, PageSize);
            return View(products);
        }
        [HttpGet]
        public IActionResult Details(int ProductId)
        {
            ShoppingCart obj = new ShoppingCart()
            {
                ProductId = ProductId,
                Product = _unitOfWork.Product.GetFirstOrDefault(x => x.Id == ProductId, includeword: "Category"),
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

            ShoppingCart Cartobj = _unitOfWork.ShoppingCart.GetFirstOrDefault(
             u => u.ApplicationUserId == claim.Value && u.ProductId == shoppingCart.ProductId
             );

            if ( Cartobj == null )
            {
                _unitOfWork.ShoppingCart.Add(shoppingCart);
                _unitOfWork.complete();
                HttpContext.Session.SetInt32(SD.SessionKey,
                    _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == claim.Value).ToList().Count()
                );
                

            }
            else
            {
                _unitOfWork.ShoppingCart.IncreaseCount(Cartobj, shoppingCart.Count);
                _unitOfWork.complete();
            }
            return RedirectToAction("Index"); 
        }
    }
}
