using Eticaret.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eticaret.Service.Abstract
{
    public interface ICartService : IService<Cart>
    {
        Task<IEnumerable<Cart>> GetCartsByUserIdAsync(int userId);
        Task<bool> AddToCartAsync(int userId, int productId, int quantity = 1);
        Task<bool> RemoveFromCartAsync(int userId, int productId);
        Task<bool> ClearCartAsync(int userId);
        Task<int> GetCartCountAsync(int userId);
    }
}
