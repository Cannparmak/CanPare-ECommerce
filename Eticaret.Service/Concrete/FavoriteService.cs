using Eticaret.Core.Entities;
using Eticaret.Data;
using Eticaret.Service.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Eticaret.Service.Concrete
{
    public class FavoriteService : Service<Favorite>, IFavoriteService
    {
        private readonly DatabaseContext _context;

        public FavoriteService(DatabaseContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Favorite>> GetFavoritesByUserIdAsync(int userId)
        {
            return await _context.Favorites
                .Include(f => f.Product)
                .Where(f => f.AppUserId == userId)
                .ToListAsync();
        }

        public async Task<bool> AddToFavoritesAsync(int userId, int productId)
        {
            // Ürünün favorilerde olup olmadığını kontrol et
            var existingFavorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.AppUserId == userId && f.ProductId == productId);

            if (existingFavorite == null)
            {
                // Yoksa yeni ekle
                var newFavorite = new Favorite
                {
                    AppUserId = userId,
                    ProductId = productId,
                    CreateDate = DateTime.Now
                };
                await _context.Favorites.AddAsync(newFavorite);
                return await _context.SaveChangesAsync() > 0;
            }

            return true; // Zaten favorilerde
        }

        public async Task<bool> RemoveFromFavoritesAsync(int userId, int productId)
        {
            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.AppUserId == userId && f.ProductId == productId);

            if (favorite != null)
            {
                _context.Favorites.Remove(favorite);
                return await _context.SaveChangesAsync() > 0;
            }

            return false;
        }

        public async Task<bool> ClearFavoritesAsync(int userId)
        {
            var favorites = await _context.Favorites
                .Where(f => f.AppUserId == userId)
                .ToListAsync();

            if (favorites.Any())
            {
                _context.Favorites.RemoveRange(favorites);
                return await _context.SaveChangesAsync() > 0;
            }

            return false;
        }

        public async Task<bool> IsProductInFavoritesAsync(int userId, int productId)
        {
            return await _context.Favorites
                .AnyAsync(f => f.AppUserId == userId && f.ProductId == productId);
        }

        public async Task<int> GetFavoritesCountAsync(int userId)
        {
            return await _context.Favorites
                .Where(f => f.AppUserId == userId)
                .CountAsync();
        }
    }
} 