using Eticaret.Core.Entities;

namespace Eticaret.Service.Abstract
{
    public interface IFavoriteService : IService<Favorite>
    {
        Task<IEnumerable<Favorite>> GetFavoritesByUserIdAsync(int userId);
        Task<bool> AddToFavoritesAsync(int userId, int productId);
        Task<bool> RemoveFromFavoritesAsync(int userId, int productId);
        Task<bool> ClearFavoritesAsync(int userId);
        Task<bool> IsProductInFavoritesAsync(int userId, int productId);
        Task<int> GetFavoritesCountAsync(int userId);
    }
} 