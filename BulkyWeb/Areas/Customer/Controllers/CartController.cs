using BulkyBook.DataAccess.Repository;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModel;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork unit;
        private readonly IEmailSender emailSender;

        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unit ,IEmailSender emailSender)
        {
            this.unit = unit;
            this.emailSender = emailSender;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM shoppingCartVM = new()
            {
                shoppingCartList = unit.ShoppingCart
                .GetAll(u => u.ApplicationUserID == userId, includeProperties: "Product"),
                orderHeader = new OrderHeader()
            };
            foreach (var cart in shoppingCartVM.shoppingCartList)
            {

                cart.Price = GetPriceBasedOnQunitity(cart);
                shoppingCartVM.orderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            return View(shoppingCartVM);
        }

        private double GetPriceBasedOnQunitity(ShoppingCart cart)
        {
            if (cart.Count <= 50)
            {
                return cart.Product.Price;
            }
            else
            {
                if (cart.Count <= 100)
                {
                    return cart.Product.Price50;
                }
                else
                {
                    return cart.Product.Price100;

                }
            }

        }


        public IActionResult Summary()
        {


            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM shoppingCartVM = new()
            {
                shoppingCartList = unit.ShoppingCart
                .GetAll(u => u.ApplicationUserID == userId, includeProperties: "Product"),
                orderHeader = new()
            };

            shoppingCartVM.orderHeader.ApplicationUser = unit.ApplicationUser.Get(u => u.Id == userId);

            shoppingCartVM.orderHeader.Name = shoppingCartVM.orderHeader.ApplicationUser.Name;
            shoppingCartVM.orderHeader.PhoneNumber = shoppingCartVM.orderHeader.ApplicationUser.PhoneNumber;
            shoppingCartVM.orderHeader.City = shoppingCartVM.orderHeader.ApplicationUser.City;
            shoppingCartVM.orderHeader.StreetAddress = shoppingCartVM.orderHeader.ApplicationUser.StreetAddress;
            shoppingCartVM.orderHeader.State = shoppingCartVM.orderHeader.ApplicationUser.State;
            shoppingCartVM.orderHeader.PostalCode = shoppingCartVM.orderHeader.ApplicationUser.PostalCode;




            foreach (var cart in shoppingCartVM.shoppingCartList)
            {

                cart.Price = GetPriceBasedOnQunitity(cart);
                shoppingCartVM.orderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            return View(shoppingCartVM);
        }

        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPost()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM.shoppingCartList = unit.ShoppingCart
                .GetAll(u => u.ApplicationUserID == userId, includeProperties: "Product");

            ShoppingCartVM.orderHeader.OrderDate = DateTime.Now;
            ShoppingCartVM.orderHeader.ApplicationUserId = userId;



            ApplicationUser applicationUser  = unit.ApplicationUser.Get(u => u.Id == userId);
            foreach (var cart in ShoppingCartVM.shoppingCartList)
            {

                cart.Price = GetPriceBasedOnQunitity(cart);
                ShoppingCartVM.orderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            if (applicationUser.CompanyID.GetValueOrDefault() == 0)
            {
                //regular customer and we need to capture payment
                ShoppingCartVM.orderHeader.OrderStatus = SD.StatusPending;
                ShoppingCartVM.orderHeader.PaymentStatus = SD.PaymentStatusPending;

            }
            else
            {
                //company  user
                ShoppingCartVM.orderHeader.OrderStatus = SD.StatusApproved;
                ShoppingCartVM.orderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;

            }
            unit.OrderHeader.Add(ShoppingCartVM.orderHeader);
            unit.Save();
            foreach (var cart in ShoppingCartVM.shoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    Count = cart.Count,
                    Price = cart.Price,
                    OrderHeaderId = ShoppingCartVM.orderHeader.Id


                };
                unit.OrderDetail.Add(orderDetail);
                unit.Save();
                
            }

            if (applicationUser.CompanyID.GetValueOrDefault() == 0)
            {
                //regular customer 
                //stripe logic
                var domain =  Request.Scheme +  "://" + Request.Host.Value +"/";
                var options = new SessionCreateOptions
                {
                    SuccessUrl =
                    domain + $"customer/cart/OrderConfirmation?id={ShoppingCartVM.orderHeader.Id}",
                    CancelUrl = domain + "customer/cart/index",
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                };
                foreach (var item in ShoppingCartVM.shoppingCartList)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100), // $20.50 => 2050
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Title
                            }
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionLineItem);
                }

                var service = new SessionService();
                Session session= service.Create(options);
                unit.OrderHeader.UpDataStripePayment(ShoppingCartVM.orderHeader.Id, session.Id, session.PaymentIntentId);
                unit.Save();

                Response.Headers.Add("Location",session.Url);
                return new StatusCodeResult(303);

            }

            return RedirectToAction(nameof(OrderConfirmation) , new{id = ShoppingCartVM.orderHeader.Id  });
        }

        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = unit.OrderHeader.Get(u => u.Id == id, includeProperties: "ApplicationUser");
            if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
                //this is a regular customer
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    unit.OrderHeader.UpDataStripePayment(id, session.Id, session.PaymentIntentId);
                    unit.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                    unit.Save();



                }
                HttpContext.Session.Clear();

            }

            //emailSender.SendEmailAsync(orderHeader.ApplicationUser.Email,"New Order Bulky Book" , 
            //    $"<p>New Order Created -{orderHeader.Id} </p>");
            List<ShoppingCart> shoppingCarts = unit.ShoppingCart.GetAll(u => u.ApplicationUserID == orderHeader.ApplicationUserId).ToList();
            unit.ShoppingCart.RemoveRange(shoppingCarts);
            unit.Save();

            return View(id);
        }
        public IActionResult Plus(int cartId)
        {

            var cartFromDB = unit.ShoppingCart.Get(u => u.Id == cartId);
            cartFromDB.Count += 1;
            unit.ShoppingCart.Update(cartFromDB);
            unit.Save();
            return RedirectToAction(nameof(Index));

        }





        public IActionResult Minus(int cartId)
        {

            var cartFromDB = unit.ShoppingCart.Get(u => u.Id == cartId , tracked :true);
            if (cartFromDB.Count <= 1)
            {
                //delete cart
               HttpContext.Session.SetInt32(SD.SessionCart 
                   ,unit.ShoppingCart.GetAll(u => u.ApplicationUserID == cartFromDB.ApplicationUserID).Count()-1);
                unit.ShoppingCart.Remove(cartFromDB);


            }
            else
            {
                cartFromDB.Count -= 1;
                unit.ShoppingCart.Update(cartFromDB);

            }
            unit.Save();
            return RedirectToAction(nameof(Index));

        }


        public IActionResult Remove(int cartId)
        {

            var cartFromDB = unit.ShoppingCart.Get(u => u.Id == cartId, tracked: true);
            HttpContext.Session.SetInt32(SD.SessionCart
    , unit.ShoppingCart.GetAll(u => u.ApplicationUserID == cartFromDB.ApplicationUserID).Count() - 1);

            unit.ShoppingCart.Remove(cartFromDB);

            unit.Save();
            return RedirectToAction(nameof(Index));

        }

    }
}
