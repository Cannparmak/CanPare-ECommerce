using Eticaret.Core.Entities;

namespace Eticaret.Service.Abstract
{
    public interface IOrderService : IService<Order>
    {
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId);
        Task<Order> GetOrderWithItemsByIdAsync(int orderId);
        Task<IEnumerable<Order>> GetOrdersWithItemsByUserIdAsync(int userId);
    }
} 