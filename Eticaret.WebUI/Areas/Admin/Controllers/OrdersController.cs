using Eticaret.Core.Entities;
using Eticaret.Core.Enums;
using Eticaret.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eticaret.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IService<OrderItem> _orderItemService;

        public OrdersController(IOrderService orderService, IService<OrderItem> orderItemService)
        {
            _orderService = orderService;
            _orderItemService = orderItemService;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _orderService.GetAllAsync();
            return View(orders);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _orderService.GetOrderWithItemsByIdAsync(id.Value);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, int status)
        {
            var order = await _orderService.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = (OrderStatus)status;
            _orderService.Update(order);
            await _orderService.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
} 