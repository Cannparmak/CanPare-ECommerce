using Eticaret.Core.Entities;
using Eticaret.Data;
using Eticaret.Service.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Eticaret.Service.Concrete
{
    public class OrderService : Service<Order>, IOrderService
    {
        private readonly DatabaseContext _context;

        public OrderService(DatabaseContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId)
        {
            return await _context.Orders
                .Where(o => o.AppUserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }
        
        public async Task<Order> GetOrderWithItemsByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }
        
        public async Task<IEnumerable<Order>> GetOrdersWithItemsByUserIdAsync(int userId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.AppUserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }
    }
} 