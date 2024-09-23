using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MyShop.Entities.Repositories;
using MyShop.Utilities;
using System.Security.Claims;

namespace MyShop.Web.View_Components
{
    public class ShoppingCartViewComponent: ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if(claim != null)
            {
                if(HttpContext.Session.GetInt32(SD.SessionKey) != null)
                {
                    return View(HttpContext.Session.GetInt32(SD.SessionKey));
                }
                else
                {
                    HttpContext.Session.SetInt32(SD.SessionKey, _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == claim.Value).ToList().Count());
                    return View(HttpContext.Session.GetInt32(SD.SessionKey));

                }
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        }
    }
}
