using Eticaret.Core.Entities;
using Eticaret.Data;
using Eticaret.Service.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Eticaret.Service.Concrete
{
    public class CartService : Service<Cart>, ICartService
    {
        private readonly DatabaseContext _context;

        public CartService(DatabaseContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Cart>> GetCartsByUserIdAsync(int userId)
        {
            return await _context.Carts
                .Include(c => c.Product)
                .Where(c => c.AppUserId == userId)
                .ToListAsync();
        }

        public async Task<bool> AddToCartAsync(int userId, int productId, int quantity = 1)
        {
            // Ürünün sepette olup olmadığını kontrol et
            var existingCartItem = await _context.Carts
                .FirstOrDefaultAsync(c => c.AppUserId == userId && c.ProductId == productId);

            if (existingCartItem != null)
            {
                // Varsa miktarını artır
                existingCartItem.Quantity += quantity;
            }
            else
            {
                // Yoksa yeni ekle
                var newCartItem = new Cart
                {
                    AppUserId = userId,
                    ProductId = productId,
                    Quantity = quantity,
                    CreateDate = DateTime.Now
                };
                await _context.Carts.AddAsync(newCartItem);
            }

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveFromCartAsync(int userId, int productId)
        {
            var cartItem = await _context.Carts
                .FirstOrDefaultAsync(c => c.AppUserId == userId && c.ProductId == productId);

            if (cartItem != null)
            {
                _context.Carts.Remove(cartItem);
                return await _context.SaveChangesAsync() > 0;
            }

            return false;
        }

        public async Task<bool> ClearCartAsync(int userId)
        {
            var cartItems = await _context.Carts
                .Where(c => c.AppUserId == userId)
                .ToListAsync();

            if (cartItems.Any())
            {
                _context.Carts.RemoveRange(cartItems);
                return await _context.SaveChangesAsync() > 0;
            }

            return false;
        }

        public async Task<int> GetCartCountAsync(int userId)
        {
            return await _context.Carts
                .Where(c => c.AppUserId == userId)
                .CountAsync();
        }
    }
}
