using Do_an_II.Hubs;
using Do_an_II.Models;
using Do_an_II.Models.ViewModels;
using Do_an_II.Repository.IRepository;
using Do_an_II.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Stripe;
using Stripe.Climate;
using System.Net.WebSockets;
using System.Security.Claims;

namespace Do_an_II.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<OrderTrackingHub> _orderHub;
        [BindProperty]
        public OrderVM OrderVM { get; set; }
        public OrderController(IUnitOfWork unitOfWork, IHubContext<OrderTrackingHub> orđerHub)
        {
            _unitOfWork = unitOfWork;
            _orderHub = orđerHub;

        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Details(int orderId)
        {
            OrderVM = new()
            {
                Order = _unitOfWork.Order.Get(u => u.Id == orderId, includeProperties: "AppUser"),
                OrderDetails = _unitOfWork.OrderDetail.GetAll(u => u.OrderId == orderId, includeProperties: "Product")
            };
           
            return View(OrderVM);
        }
        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        public IActionResult UpdateOrderDetail()
        {
            var orderDb = _unitOfWork.Order.Get(u => u.Id == OrderVM.Order.Id);
            orderDb.Name = OrderVM.Order.Name;
            orderDb.PhoneNumber = OrderVM.Order.PhoneNumber;
            orderDb.Address = OrderVM.Order.Address;
            orderDb.City = OrderVM.Order.City;
            orderDb.State = OrderVM.Order.State;
            orderDb.PostalCode = OrderVM.Order.PostalCode;
            if (!string.IsNullOrEmpty(OrderVM.Order.TrackingNumber))
            {
                orderDb.TrackingNumber = OrderVM.Order.TrackingNumber;
            }
            if (!string.IsNullOrEmpty(OrderVM.Order.Carrier))
            {
                orderDb.Carrier = OrderVM.Order.Carrier;
            }
            _unitOfWork.Order.Update(orderDb);
            _unitOfWork.Save();
            TempData["success"] = "Cập nhật đơn hàng thành công";
            return RedirectToAction(nameof(Details), new { orderId = orderDb.Id });
        }

        [HttpPost]
        public IActionResult StartProcessing()
        {
            var order = _unitOfWork.Order.Get(u => u.Id == OrderVM.Order.Id);
            _unitOfWork.Order.UpdateStatus(OrderVM.Order.Id, Status.StatusInProcess);
            _unitOfWork.Save();
            _orderHub.Clients.User(order.AppUserId)
            .SendAsync("ReceiveOrderUpdate", order.Id, order.OrderStatus);
            TempData["success"] = "Đơn hàng đang được xử lý";
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.Order.Id });
        }
        [HttpPost]
        public IActionResult ShipOrder()
        {
            var order = _unitOfWork.Order.Get(u => u.Id == OrderVM.Order.Id);
            order.TrackingNumber = OrderVM.Order.TrackingNumber;
            order.Carrier = OrderVM.Order.Carrier;
            order.OrderStatus = Status.StatusShipped;
            order.ShippingDate = DateTime.Now;
            if (order.PaymentStatus == Status.PaymentStatusDelayedPayment)
            {
                order.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
            }
            _unitOfWork.Order.Update(order);
            _unitOfWork.Save();
            _orderHub.Clients.User(order.AppUserId)
            .SendAsync("ReceiveOrderUpdate", order.Id, order.OrderStatus);
            TempData["success"] = "Đơn hàng đã được giao";
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.Order.Id });
        }
        [HttpPost]
        public IActionResult CancelOrder()
        {
            var order = _unitOfWork.Order.Get(u => u.Id == OrderVM.Order.Id);
            if (order.PaymentStatus == Status.PaymentStatusApproved)
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = order.PaymentIntentId,
                };
                var service = new RefundService();
                Refund refund = service.Create(options);
                _unitOfWork.Order.UpdateStatus(order.Id, Status.StatusCancelled, Status.StatusRefunded);
            }
            else
            {
                _unitOfWork.Order.UpdateStatus(order.Id, Status.StatusCancelled, Status.StatusCancelled);
            }
            _unitOfWork.Save();
            _orderHub.Clients.User(order.AppUserId)
            .SendAsync("ReceiveOrderUpdate", order.Id, order.OrderStatus);
            TempData["success"] = "Đơn hàng đã bị hủy";


            return RedirectToAction(nameof(Details), new { orderId = OrderVM.Order.Id });
        }

            #region API CALLs

            [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<Models.Order> orderList;
            if (User.IsInRole(Roles.Admin) || User.IsInRole(Roles.Employee))    
            {
                orderList = _unitOfWork.Order.GetAll(includeProperties: "AppUser").ToList();
            }
            else {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                orderList = _unitOfWork.Order.GetAll(u => u.AppUserId == userId, includeProperties: "AppUser").ToList();
            }
                switch (status)
                {
                    case "pending":
                        orderList = orderList.Where(o => o.OrderStatus == Status.PaymentStatusPending);
                        break;
                    case "inprocess":
                        orderList = orderList.Where(o => o.OrderStatus == Status.StatusInProcess);
                        break;
                    case "completed":
                        orderList = orderList.Where(o => o.OrderStatus == Status.StatusShipped);
                        break;
                    case "approved":
                        orderList = orderList.Where(o => o.OrderStatus == Status.StatusApproved);
                        break;
                    default:
                        break;
                }
            return Json(new { data = orderList });
        }
        #endregion
    }
}
