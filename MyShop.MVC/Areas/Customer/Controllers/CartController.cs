using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyShop.Entities.Models;
using MyShop.Entities.Repositories;
using MyShop.Entities.ViewModels;
using MyShop.Utilities;
using Stripe.Checkout;
using System.Security.Claims;

namespace MyShop.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public  ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [Authorize]
        public IActionResult Index()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new ShoppingCartVM()
            {
                CartsList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeword:"Product")
            };

            foreach(var item in ShoppingCartVM.CartsList)
            {
                ShoppingCartVM.TotalCarts += (item.Count * item.Product.Price);
            }
            return View(ShoppingCartVM);
        }


        [HttpGet]
		public IActionResult Summary()
		{
            var claimIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

			ShoppingCartVM = new ShoppingCartVM()
			{
				CartsList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeword: "Product"),
			    OrderHeader = new()
            };

            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(x => x.Id == claim.Value);
            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.Address = ShoppingCartVM.OrderHeader.ApplicationUser.Address;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.Phone = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;


            foreach (var item in ShoppingCartVM.CartsList)
			{
				ShoppingCartVM.OrderHeader.TotalPrice += (item.Count * item.Product.Price);
			}
			return View(ShoppingCartVM); 
 
		}


        [HttpPost]
        [ActionName("Summary")]
        [ValidateAntiForgeryToken]
        public IActionResult POSTSummary(ShoppingCartVM ShoppingCartVM)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);


            ShoppingCartVM.CartsList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeword: "Product");
            

            ShoppingCartVM.OrderHeader.OrderStatus = SD.Pending;
            ShoppingCartVM.OrderHeader.PaymentStatus = SD.Pending;
            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;
 

            foreach (var item in ShoppingCartVM.CartsList)
            {
                ShoppingCartVM.OrderHeader.TotalPrice += (item.Count * item.Product.Price);
            }
            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.complete();

            foreach(var item in ShoppingCartVM.CartsList)
            {
                OrderDetail orderDetail = new OrderDetail()
                {
                    ProductId = item.ProductId,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    Price = item.Product.Price,
                    Count = item.Count,
                    
                };

                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.complete();
            }
            var domain = "https://localhost:7039/";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + $"Customer/Cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
                CancelUrl = domain + $"Customer/Cart/Index"
            };

            foreach(var item in ShoppingCartVM.CartsList)
            {
               var sessionLineOptions =  new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Product.Price * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name,
                        },
                    },

                    Quantity = item.Count,
                };
                options.LineItems.Add(sessionLineOptions);
            }
            var service = new SessionService();
            Session session = service.Create(options);

            ShoppingCartVM.OrderHeader.SessionId = session.Id;
            _unitOfWork.complete();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);

        }
        public IActionResult OrderConfirmation([FromQuery] int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id);
            var service = new SessionService();
            Session session = service.Get(orderHeader.SessionId);
            if (session.PaymentStatus.ToLower() == "paid")
            {
                _unitOfWork.OrderHeader.UpdateOrderStatus(id, SD.Approved, SD.Approved);
               orderHeader.PaymentIntentId = session.PaymentIntentId;

            }
            List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart.GetAll(u=>u.ApplicationUserId==orderHeader.ApplicationUserId).ToList();
            _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
            _unitOfWork.complete();
            return View(id);
        }
        public IActionResult Plus([FromQuery]int cartId)
        {
            var shoppingcart = _unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.Id == cartId);
            _unitOfWork.ShoppingCart.IncreaseCount(shoppingcart, 1);
            _unitOfWork.complete();
            return RedirectToAction("Index");
        }

		public IActionResult Minus([FromQuery] int cartId)
		{
			var shoppingcart = _unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.Id == cartId);
			if(shoppingcart.Count <= 1)
            {
                _unitOfWork.ShoppingCart.Remove(shoppingcart);
                var count = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == shoppingcart.ApplicationUserId).ToList().Count() - 1;
                HttpContext.Session.SetInt32(SD.SessionKey, count);

            }
            else
            {
                _unitOfWork.ShoppingCart.DecreaseCount(shoppingcart, 1);
            }
            
            _unitOfWork.ShoppingCart.DecreaseCount(shoppingcart, 1);
			_unitOfWork.complete();
			return RedirectToAction("Index");
		}

		public IActionResult Remove([FromQuery] int cartId)
		{
			var shoppingcart = _unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.Id == cartId);
            _unitOfWork.ShoppingCart.Remove(shoppingcart);

            var count = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == shoppingcart.ApplicationUserId).ToList().Count();
            HttpContext.Session.SetInt32(SD.SessionKey, count);
            _unitOfWork.complete();
			return RedirectToAction("Index");
		}
	}
}
