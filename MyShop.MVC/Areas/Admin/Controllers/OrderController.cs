using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShop.DataAccess.Implementations;
using MyShop.Entities.Models;
using MyShop.Entities.Repositories;
using MyShop.Entities.ViewModels;
using MyShop.Utilities;
using Stripe;

namespace MyShop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminRole)]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public OrderVM OrderVM { get; set; }
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetData()
        {
           
            var orderHeaders= _unitOfWork.OrderHeader.GetAll(includeword: "ApplicationUser");
            return Json(new { data = orderHeaders });
        }

        [HttpGet]
        public IActionResult Details([FromQuery]int orderId)
        {
            OrderVM orderVM = new OrderVM()
            {
                OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == orderId, includeword: "ApplicationUser"),
                OrderDetails = _unitOfWork.OrderDetail.GetAll(x => x.OrderHeaderId == orderId, includeword: "Product")
            };
            return View(orderVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateOrderDetails()
        {
            var orderFromDb = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id);
            orderFromDb.Name = OrderVM.OrderHeader.Name;
            orderFromDb.Address = OrderVM.OrderHeader.Address;
            orderFromDb.Phone = OrderVM.OrderHeader.Phone;
            orderFromDb.City = OrderVM.OrderHeader.City;

            if(OrderVM.OrderHeader.Carrier != null)
            {
                orderFromDb.Carrier = OrderVM.OrderHeader.Carrier;  
            }
            if(OrderVM.OrderHeader.TrackingNumber != null)
            {
                orderFromDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            }

            _unitOfWork.OrderHeader.Update(orderFromDb);
            _unitOfWork.complete();

            TempData["Update"] = "Data Has Updated Successfully";
            return RedirectToAction("Details", "Order" , new {orderId = orderFromDb.Id});

           
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StartProcess()
        {
            _unitOfWork.OrderHeader.UpdateOrderStatus(OrderVM.OrderHeader.Id, SD.Processing, null);
            _unitOfWork.complete();
              
            TempData["Update"] = "Order Status Has Updated Successfully";
            return RedirectToAction("Details", "Order", new { orderId = OrderVM.OrderHeader.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StartShip()
        {
            var orderFromDb = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id);
            orderFromDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            orderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
            orderFromDb.OrderStatus = SD.Shipped;
            orderFromDb.ShippingDate = DateTime.Now;

            _unitOfWork.OrderHeader.Update(orderFromDb);
            _unitOfWork.complete();

            TempData["Update"] = "Order Has Shipped Successfully";
            return RedirectToAction("Details", "Order", new { orderId = OrderVM.OrderHeader.Id });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelOrder()
        {
            var orderFromDb = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id);
            if (orderFromDb.OrderStatus == SD.Approved)
            {
                var option = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderFromDb.PaymentIntentId,
                };

                var service = new RefundService();
                Refund refund = service.Create(option);
                
                _unitOfWork.OrderHeader.UpdateOrderStatus(orderFromDb.Id, SD.Cancelled, SD.Refund);
            }
            else
            {
                _unitOfWork.OrderHeader.UpdateOrderStatus(orderFromDb.Id, SD.Cancelled, SD.Cancelled);

            }

            _unitOfWork.complete();
            TempData["Update"] = "Order Has Cancelled Successfully";
            return RedirectToAction("Details", "Order", new { orderId = OrderVM.OrderHeader.Id });
        }
    }
}
