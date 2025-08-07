using Do_an_II.Models;
using Do_an_II.Models.ViewModels;
using Do_an_II.Repository.IRepository;
using Do_an_II.Services.EmailServices;
using Do_an_II.Services.VnPay;
using Do_an_II.Utilities;
using Do_an_II.Utilities.VNPay;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Stripe.Checkout;
using System.Security.Claims;

namespace Do_an_II.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IVnPayService _vnPayService;
        [BindProperty]
        public CartVM CartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork, IEmailService emailService, IHubContext<NotificationHub> hubContext, IVnPayService vnPayService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _hubContext = hubContext;
           _vnPayService = vnPayService;
        }


        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            CartVM = new()
            {
                CartList = _unitOfWork.Cart.GetAll(u => u.AppUserId == userId, includeProperties: "Product"),
                Order = new()
            };

            IEnumerable<ProductImage> productImages = _unitOfWork.ProductImage.GetAll();
            

            foreach (var cart in CartVM.CartList)
            {
                cart.Product.ProductImages = productImages.Where(u => u.ProductId == cart.ProductId).ToList();
                cart.Price = GetOrderTotal(cart);
                CartVM.Order.OrderTotal += (cart.Count * cart.Price);
            }
            return View(CartVM);

        }
        [HttpGet]
        public IActionResult Checkout()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            CartVM = new CartVM()
            {
                CartList = _unitOfWork.Cart.GetAll(u => u.AppUserId == userId, includeProperties: "Product"),
                Order = new Order()
            };

            CartVM.Order.AppUser = _unitOfWork.AppUser.Get(u => u.Id == userId);
            CartVM.Order.Name = CartVM.Order.AppUser.Name;
            CartVM.Order.PhoneNumber = CartVM.Order.AppUser.PhoneNumber;
            CartVM.Order.Address = CartVM.Order.AppUser.Address;
            CartVM.Order.City = CartVM.Order.AppUser.City;
            CartVM.Order.State = CartVM.Order.AppUser.State;
            CartVM.Order.PostalCode = CartVM.Order.AppUser.PostalCode;

            foreach (var cart in CartVM.CartList)
            {
                cart.Price = GetOrderTotal(cart);
                CartVM.Order.OrderTotal += (cart.Count * cart.Price);
            }

            return View(CartVM); // Trả về view hiện summary cho người dùng
        }

        [HttpPost]
        [ActionName("Checkout")]
        public IActionResult CheckoutPOST(string paymentMethod)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            CartVM.CartList = _unitOfWork.Cart.GetAll(u => u.AppUserId == userId, includeProperties: "Product");
            CartVM.Order.OrderDate = System.DateTime.Now;
            CartVM.Order.AppUserId = userId;

            AppUser appUser = _unitOfWork.AppUser.Get(u => u.Id == userId);



            foreach (var cart in CartVM.CartList)
            {
                cart.Price = GetOrderTotal(cart);
                CartVM.Order.OrderTotal += (cart.Count * cart.Price);
            }

            CartVM.Order.PaymentStatus = Status.StatusPending;
            CartVM.Order.OrderStatus = Status.StatusPending;

            _unitOfWork.Order.Add(CartVM.Order);
            _unitOfWork.Save();
            foreach (var cart in CartVM.CartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderId = CartVM.Order.Id,
                    Count = cart.Count,
                    Price = cart.Price
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }



            if (paymentMethod == "stripe")
            {
                var domain = "https://localhost:7085/";
                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    SuccessUrl = domain + $"Customer/Cart/OrderConfirmation?id={CartVM.Order.Id}",
                    CancelUrl = domain + "Customer/Cart/Index",
                    LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                    Mode = "payment",




                };
                foreach (var item in CartVM.CartList)
                {
                    var SessionlineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Name,
                            }

                        },
                        Quantity = item.Count,
                    };
                    options.LineItems.Add(SessionlineItem);
                }
                var service = new SessionService();
                Session session = service.Create(options);
                _unitOfWork.Order.UpdateStripePaymentID(CartVM.Order.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.Order.UpdateStatus(CartVM.Order.Id, Status.StatusPending, Status.PaymentStatusPending);
                _unitOfWork.Save();
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }
            else if (paymentMethod == "vnpay")
            {
                var model = new VnPayRequestModel
                {
                    OrderId = CartVM.Order.Id,
                    Fullname = appUser.Name,
                    Description = "Thanh toán đơn hàng " + CartVM.Order.Id,
                    Amount = CartVM.Order.OrderTotal,
                    CreatedDate = DateTime.Now
                };
                string paymentUrl = _vnPayService.CreatePaymentUrl(model, HttpContext);
                return Redirect(paymentUrl);
            }

            //var noti = new Notification
            //{
            //    Message = $"Khách {User.Identity.Name} vừa đặt hàng lúc {DateTime.Now:HH:mm}",
            //    UserId = null // hoặc Id của admin
            //};
            //_unitOfWork.Notification.Add(noti);
            //_unitOfWork.Save();

            // Gửi SignalR
            //_hubContext.Clients.Group("Admins").SendAsync("ReceiveNotification", noti.Message);
            else
            {
                return RedirectToAction(nameof(OrderConfirmation), new { id = CartVM.Order.Id });
            }
        }

        public IActionResult OrderConfirmation(int id)
        {
            Order order = _unitOfWork.Order.Get(u => u.Id == id, includeProperties: "AppUser");
            if (order == null)
            {
                return NotFound();
            }
            if (order.PaymentMethod == PaymentMethod.Stripe)
            {
                
                    var service = new SessionService();
                    Session session = service.Get(order.SessionId);
                    // Order is not delayed payment
                    if (session.PaymentStatus.ToLower() == "paid")
                    {
                        _unitOfWork.Order.UpdateStripePaymentID(id, session.Id, session.PaymentIntentId);
                        _unitOfWork.Order.UpdateStatus(id, Status.StatusApproved, Status.PaymentStatusApproved);
                        _unitOfWork.Save();
                    }


                
            }
            else if (order.PaymentMethod == PaymentMethod.VnPay)
            {
            
            }
            else if (order.PaymentMethod == PaymentMethod.Cod)
            {
                _unitOfWork.Order.UpdateStatus(id, Status.StatusPending, Status.PaymentStatusPending);
                _unitOfWork.Save();
            }

            //_emailSender.SendEmailAsync(order.AppUser.Email, "Đơn hàng mới - Eshop",
            //    $"<p>Đơn hàng -{order.Id} đã được xác nhận. Cảm ơn bạn đã mua sắm tại cửa hàng của chúng tôi!</p>");

            List<Cart> carts = _unitOfWork.Cart.GetAll(u => u.AppUserId == order.AppUserId).ToList();
            _unitOfWork.Cart.RemoveRange(carts);
            _unitOfWork.Save();

            HttpContext.Session.Clear();
            _emailService.SendEmailAsync(order.AppUser.Email, "Đơn hàng mới - Eshop",
                $"<p>Đơn hàng -{order.Id} đã được xác nhận. Cảm ơn bạn đã mua sắm tại cửa hàng của chúng tôi!</p>");

            return View(id);
        }




        private double GetOrderTotal(Cart cart)
        {
            return (double)cart.Product.Price;
        }

        public IActionResult Plus(int cartId)
        {
            var cart = _unitOfWork.Cart.Get(u => u.Id == cartId);
            cart.Count += 1;
            _unitOfWork.Cart.Update(cart);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            var cart = _unitOfWork.Cart.Get(u => u.Id == cartId);
            if (cart.Count <= 1)
            {
                _unitOfWork.Cart.Remove(cart);
                HttpContext.Session.SetInt32(SessionSettings.SessionCart, _unitOfWork.Cart
                   .GetAll(u => u.AppUserId == cart.AppUserId).Count() - 1);
            }
            else
            {
                cart.Count -= 1;
                _unitOfWork.Cart.Update(cart);
            }

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Remove(int cartId)
        {
            var cart = _unitOfWork.Cart.Get(u => u.Id == cartId,tracked:true);

            
            HttpContext.Session.SetInt32(SessionSettings.SessionCart, _unitOfWork.Cart.GetAll(u => u.AppUserId == cart.AppUserId).Count() - 1);
            _unitOfWork.Cart.Remove(cart);
            _unitOfWork.Save();
            TempData["success"] = "Xóa sản phẩm thành công";
            return RedirectToAction(nameof(Index));
        }


    }
}
