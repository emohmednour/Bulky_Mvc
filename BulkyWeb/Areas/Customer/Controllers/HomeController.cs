using System.Diagnostics;
using System.Security.Claims;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork unit;

        public HomeController(ILogger<HomeController> logger , IUnitOfWork unit)
        {
            _logger = logger;
            this.unit = unit;
        }

        public IActionResult Index()
        {

           
            IEnumerable<Product> products= unit.Product.GetAll(includeProperties : "Category,ProductImages");
            return View(products);
        }
        public IActionResult Details(int productid)
        {
            ShoppingCart cart = new()
            {
                Product = unit.Product.Get(u =>u.Id == productid, includeProperties: "Category,ProductImages"),
                Count= 1,
                ProductId = productid
            };
                
            return View(cart);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userid = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserID = userid;

             var CartFromDB = unit.ShoppingCart.Get(u => u.ApplicationUserID==userid &&  u.ProductId == shoppingCart.ProductId);
            if (CartFromDB != null)
            {
                //update cart
                CartFromDB.Count += shoppingCart.Count;
                unit.ShoppingCart.Update(CartFromDB);
            unit.Save();
            }
            else
            {
                //add cart
            unit.ShoppingCart.Add(shoppingCart);
            unit.Save();
                HttpContext.Session.SetInt32(SD.SessionCart,
                    unit.ShoppingCart.GetAll(u => u.ApplicationUserID == userid).Count());
            }

            return RedirectToAction(nameof(Index));
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
