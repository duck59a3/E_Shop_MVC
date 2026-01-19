using Do_an_II.Hubs;
using Do_an_II.Messagings.Messages;
using Do_an_II.Models;
using Do_an_II.Models.ViewModels;
using Do_an_II.Repository.IRepository;
using Do_an_II.Services.EmailServices;
using Do_an_II.Services.RabbitMQServices;
using Do_an_II.Services.RedisServices;
using Do_an_II.Services.VnPay;
using Do_an_II.Utilities;
using Do_an_II.Utilities.VNPay;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Stripe.Checkout;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Do_an_II.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IHubContext<DashboardHub> _dashboardHubContext;
        private readonly IHubContext<NotifyHub> _NotifyHubContext;
        private readonly IVnPayService _vnPayService;
        private readonly IRedisService _cache;
        private readonly OrderPublisher _orderPublisher;
        private readonly ILogger<CartController> _logger;

        [BindProperty]
        public CartVM CartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork,
            IEmailService emailService,
            IVnPayService vnPayService,
            IHubContext<DashboardHub> dashboardHubContext,
            IHubContext<NotifyHub> notifyHubContext,
            IRedisService redisService,
            OrderPublisher orderPublisher,
            ILogger<CartController> logger)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _vnPayService = vnPayService;
            _dashboardHubContext = dashboardHubContext;
            _NotifyHubContext = notifyHubContext;
            _cache = redisService;
            _orderPublisher = orderPublisher;
            _logger = logger;
        }


        public async Task<IActionResult> Index()
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
                var flashSaleId = await _cache.GetActiveFlashSaleIdAsync(cart.ProductId);
                var flashPrice = await _cache.GetFlashSalePriceAsync(flashSaleId,cart.ProductId);
                var flashQty = await _cache.GetFlashSaleStockAsync(flashSaleId, cart.ProductId);
                var flashEndTime = await _cache.GetFlashSaleEndTimeAsync(flashSaleId,cart.ProductId);

                bool isFlashSaleActive = flashPrice.HasValue
                    && flashQty > 0
                    && flashEndTime.HasValue
                    && flashEndTime.Value > DateTimeOffset.UtcNow;

                cart.Price = isFlashSaleActive ? (double)flashPrice.Value : GetOrderTotal(cart);
                CartVM.Order.OrderTotal += (cart.Count * cart.Price);
            }
            return View(CartVM);

        }
        [HttpGet]
        public async Task<IActionResult> Checkout()
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
                var flashSaleId = await _cache.GetActiveFlashSaleIdAsync(cart.ProductId);
                var flashPrice = await _cache.GetFlashSalePriceAsync(flashSaleId,cart.ProductId);
                bool isFlashSaleActive = flashPrice.HasValue;
                cart.Price = isFlashSaleActive ? (double)flashPrice.Value : GetOrderTotal(cart);
                CartVM.Order.OrderTotal += (cart.Count * cart.Price);
            }

            return View(CartVM); // Trả về view hiện summary cho người dùng
        }

        [HttpPost]
        [ActionName("Checkout")]
        public async Task<IActionResult> CheckoutPOST(string paymentMethod)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            CartVM.CartList = _unitOfWork.Cart.GetAll(u => u.AppUserId == userId, includeProperties: "Product");
            CartVM.Order.OrderDate = DateTime.Now;
            CartVM.Order.AppUserId = userId;

            AppUser appUser = _unitOfWork.AppUser.Get(u => u.Id == userId);

            var flashSaleInfo = new Dictionary<int, (bool isActive, string? flashSaleId)>();

            foreach (var cart in CartVM.CartList)
            {
                var flashSaleId = await _cache.GetActiveFlashSaleIdAsync(cart.ProductId);
                var flashPrice = await _cache.GetFlashSalePriceAsync(flashSaleId,cart.ProductId);
                var flashQty = await _cache.GetFlashSaleStockAsync(flashSaleId, cart.ProductId);
                var flashEndTime = await _cache.GetFlashSaleEndTimeAsync(flashSaleId, cart.ProductId);
                

                bool isFlashSaleActive =
                    flashSaleId != null &&
                    flashPrice.HasValue &&
                    flashQty > 0 &&
                    flashEndTime.HasValue &&
                    flashEndTime.Value > DateTimeOffset.UtcNow;

                flashSaleInfo[cart.ProductId] = (isFlashSaleActive, flashSaleId);
                cart.Price = isFlashSaleActive
                    ? (double)flashPrice.Value
                    : (double)cart.Product.Price;
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
                    Price = cart.Price,
                    IsFlashSale = flashSaleInfo[cart.ProductId].isActive,
                    FlashSaleId = flashSaleInfo[cart.ProductId].flashSaleId
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
               
            }
            _unitOfWork.Save();
            bool hasFlashSale = flashSaleInfo.Values.Any(info => info.isActive);
            if (hasFlashSale)
            {
                var items = await Task.WhenAll(CartVM.CartList.Select(async cart =>
                {
                    var flashSaleId = await _cache.GetActiveFlashSaleIdAsync(cart.ProductId);
                    var flashPrice = await _cache.GetFlashSalePriceAsync(flashSaleId,cart.ProductId);
                   

                    return new OrderItemMessage
                    {
                        ProductId = cart.ProductId,
                        Quantity = cart.Count,
                        Price = (decimal)cart.Price,
                        IsFlashSale = flashSaleInfo[cart.ProductId].isActive,
                        FlashSaleId = flashSaleId!
                    };
                }));

                //await _orderPublisher.PublishOrderCreatedAsync(
                //    new OrderCreated
                //    {
                //        OrderId = CartVM.Order.Id,
                //        UserId = userId,
                //        Items = items.ToList()
                //    }
                //);

            }
            await _dashboardHubContext.Clients.All.SendAsync("UpdateDashboard", new
            {
                order = _unitOfWork.Dashboard.GetTotalOrdersToday(),
                revenue = _unitOfWork.Dashboard.GetTotalRevenueToday(),
                allorder = _unitOfWork.Dashboard.GetTotalOrders(),
                allrevenue = _unitOfWork.Dashboard.GetTotalRevenue()

            });
            await PublishFlashSaleOrderAsync(CartVM.Order.Id, userId);

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
                CartVM.Order.PaymentMethod = PaymentMethod.VnPay;
                _unitOfWork.Order.Update(CartVM.Order);
                _unitOfWork.Save();

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

            // _NotifyHubContext.Clients.Group("Admins").SendAsync("ReceiveOrderNotification", new
            //{
            //    orderId = CartVM.Order.Id,
            //    message = $"🛒 Đơn hàng mới #{CartVM.Order.Id} từ khách hàng {CartVM.Order.AppUser.Name}"
            //});
            return RedirectToAction(nameof(OrderConfirmation), new { id = CartVM.Order.Id });


        }
        [HttpGet]
        public async Task<IActionResult> PaymentCallbackVnpay()
        {
            var response = _vnPayService.PaymentExcute(Request.Query);

            if (response == null || !response.Success)
            {
                TempData["error"] = "Thanh toán VNPay thất bại!";
                return RedirectToAction(nameof(Index));
            }

            var orderId = int.Parse(response.OrderId);
            var order = _unitOfWork.Order.Get(u => u.Id == orderId, includeProperties: "AppUser");

            if (order == null)
            {
                TempData["error"] = "Không tìm thấy đơn hàng!";
                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra mã response từ VNPay
            if (response.VnPayResponseCode == "00") // Thanh toán thành công
            {
                // Cập nhật thông tin thanh toán
                _unitOfWork.Order.UpdateStatus(orderId, Status.StatusApproved, Status.PaymentStatusApproved);
                _unitOfWork.Save();

                return RedirectToAction(nameof(OrderConfirmation), new { id = orderId });
            }
            else
            {
                // Thanh toán thất bại - hủy đơn hàng
                _unitOfWork.Order.UpdateStatus(orderId, Status.StatusCancelled, Status.PaymentStatusRejected);
                _unitOfWork.Save();

                TempData["error"] = "Thanh toán không thành công. Mã lỗi: " + response.VnPayResponseCode;
                return RedirectToAction(nameof(Index));
            }
        }
        public async Task<IActionResult> OrderConfirmation(int id)
        {
            Order order = _unitOfWork.Order.Get(u => u.Id == id, includeProperties: "AppUser");
            var orderDetails = _unitOfWork.OrderDetail.GetAll(u => u.OrderId == id, includeProperties: "Product").ToList();
            if (order == null)
            {
                return NotFound();
            }
            bool isPaid = false;
            if (order.PaymentMethod == PaymentMethod.Stripe)
            {

                var service = new SessionService();
                Session session = service.Get(order.SessionId);
                // Order is not delayed payment
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    isPaid = true;
                    _unitOfWork.Order.UpdateStripePaymentID(id, session.Id, session.PaymentIntentId);
                    _unitOfWork.Order.UpdateStatus(id, Status.StatusApproved, Status.PaymentStatusApproved);
                    _unitOfWork.Save();
                    await _orderPublisher.PublishOrderConfirmedAsync(
                     new OrderConfirmed
                     {
                         OrderId = order.Id,
                         UserId = order.AppUserId,
                         Items = orderDetails.Select(od => new OrderItemMessage
                         {
                             ProductId = od.ProductId,
                             Quantity = od.Count,
                             IsFlashSale = od.IsFlashSale,
                             FlashSaleId = od.FlashSaleId!
                         }).ToList()
                     }
                 );
                }
                else
                {
                    _unitOfWork.Order.UpdateStatus(id, Status.StatusCancelled, Status.PaymentStatusRejected);
                    //await _orderPublisher.PublishOrderCancelledAsync(
                    // new OrderCancelled
                    // {
                    //     OrderId = order.Id,
                    //     UserId = order.AppUserId,
                    //     Items = orderDetails.Select(od => new OrderItemMessage
                    //     {
                    //         ProductId = od.ProductId,
                    //         Quantity = od.Count,
                    //         IsFlashSale = od.IsFlashSale,
                    //         FlashSaleId = od.FlashSaleId!
                    //     }).ToList()
                    // }
                }



            }
            else if (order.PaymentMethod == PaymentMethod.VnPay)
            {
                // Kiểm tra xem đã thanh toán chưa dựa vào PaymentStatus
                if (order.PaymentStatus == Status.PaymentStatusApproved)
                {
                    isPaid = true;
                    var orderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderId == id, includeProperties: "Product").ToList();

                    await _orderPublisher.PublishOrderConfirmedAsync(
                        new OrderConfirmed
                        {
                            OrderId = order.Id,
                            UserId = order.AppUserId,
                            Items = orderDetail.Select(od => new OrderItemMessage
                            {
                                ProductId = od.ProductId,
                                Quantity = od.Count,
                                IsFlashSale = od.IsFlashSale,
                                FlashSaleId = od.FlashSaleId!
                            }).ToList()
                        }
                    );
                }
                else if (order.PaymentStatus == Status.PaymentStatusRejected)
                {
                    // Thanh toán bị từ chối
                    isPaid = false;
                }
            }
            else if (order.PaymentMethod == PaymentMethod.Cod)
            {
                isPaid = true;
                _unitOfWork.Order.UpdateStatus(id, Status.StatusPending, Status.PaymentStatusPending);
                _unitOfWork.Save();
            }

            
            
            List<Cart> carts = _unitOfWork.Cart.GetAll(u => u.AppUserId == order.AppUserId).ToList();
            _unitOfWork.Cart.RemoveRange(carts);
            var user = _unitOfWork.AppUser.Get(u => u.Id == order.AppUserId);
            user.TotalOrders += 1;
            user.TotalSpent += order.OrderTotal;
            if (user.TotalOrders >= 50 && user.TotalSpent >= 10000000)
            {
                user.MemberLevel = MemberLevel.Diamond;
            }
            else if (user.TotalOrders >= 30 && user.TotalSpent >= 5000000)
            {
                user.MemberLevel = MemberLevel.Platinum;
            }
            else if (user.TotalOrders >= 20 && user.TotalSpent >= 2000000)
            {
                user.MemberLevel = MemberLevel.Gold;
            }
            else if (user.TotalOrders >= 10 && user.TotalSpent >= 1000000)
            {
                user.MemberLevel = MemberLevel.Silver;
            }
            _unitOfWork.AppUser.Update(user);
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
            var cart = _unitOfWork.Cart.Get(u => u.Id == cartId, tracked: true);


            HttpContext.Session.SetInt32(SessionSettings.SessionCart, _unitOfWork.Cart.GetAll(u => u.AppUserId == cart.AppUserId).Count() - 1);
            _unitOfWork.Cart.Remove(cart);
            _unitOfWork.Save();
            TempData["success"] = "Xóa sản phẩm thành công";
            return RedirectToAction(nameof(Index));
        }
        private async Task PublishFlashSaleOrderAsync(int orderId, string userId)
        {
            try
            {
                var orderDetails = _unitOfWork.OrderDetail
                    .GetAll(u => u.OrderId == orderId, includeProperties: "Product")
                    .ToList();

                var flashSaleItems = orderDetails
                    .Where(od => od.IsFlashSale)
                    .Select(od => new OrderItemMessage
                    {
                        ProductId = od.ProductId,
                        Quantity = od.Count,
                        Price = (decimal)od.Price,
                        IsFlashSale = true,
                        FlashSaleId = od.FlashSaleId
                    })
                    .ToList();

                if (flashSaleItems.Any())
                {
                    await _orderPublisher.PublishOrderCreatedAsync(
                        new OrderCreated
                        {
                            OrderId = orderId,
                            UserId = userId,
                            Items = flashSaleItems,
                            TotalAmount = (decimal)orderDetails.Sum(x => x.Price * x.Count),

                            
                        }
                    );

                    _logger.LogInformation(
                        "Published flash sale order {OrderId} with {Count} items",
                        orderId, flashSaleItems.Count
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish flash sale order {OrderId}", orderId);
                // Không throw để không ảnh hưởng flow chính
            }
        }



    }
}
