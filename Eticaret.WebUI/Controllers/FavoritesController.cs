using Eticaret.Core.Entities;
using Eticaret.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Eticaret.WebUI.Controllers
{
    [Authorize] // Bu controller'daki tüm action'lar için yetkilendirme gereksin
    public class FavoritesController : Controller
    {
        private readonly IFavoriteService _favoriteService;
        private readonly IService<Product> _productService; // Ürün detayları için

        public FavoritesController(IFavoriteService favoriteService, IService<Product> productService)
        {
            _favoriteService = favoriteService;
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToAction("SignIn", "Account");
            }

            int userId = int.Parse(userIdClaim);
            var favoriteItems = await _favoriteService.GetFavoritesByUserIdAsync(userId);
            return View(favoriteItems);
        }

        [HttpPost] // HTTP POST isteği olarak işaretlendi
        public async Task<IActionResult> AddToFavorites(int productId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                // AJAX isteği kontrolü
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Lütfen giriş yapınız." });
                }
                return RedirectToAction("SignIn", "Account");
            }
            int userId = int.Parse(userIdClaim);

            var product = await _productService.GetAsync(p => p.Id == productId);
            if (product == null)
            {
                // AJAX isteği kontrolü
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Ürün bulunamadı." });
                }
                return RedirectToAction("Index", "Home");
            }

            await _favoriteService.AddToFavoritesAsync(userId, productId);
            
            // Favori sayısını al
            var favoriteCount = await _favoriteService.GetFavoritesCountAsync(userId);
            
            // AJAX isteği kontrolü
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true, message = "Favorilere eklendi.", favoriteCount });
            }
            
            // Normal istek ise sayfaya yönlendir
            TempData["Success"] = "Ürün favorilerinize eklendi.";
            return RedirectToAction("Index");
        }

        [HttpPost] // HTTP POST isteği olarak işaretlendi
        public async Task<IActionResult> RemoveFromFavorites(int productId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Json(new { success = false, message = "Lütfen giriş yapınız." });
            }
            int userId = int.Parse(userIdClaim);

            await _favoriteService.RemoveFromFavoritesAsync(userId, productId);
            // UI'ı güncellemek için favori sayısını da dönebiliriz
            var favoriteCount = await _favoriteService.GetFavoritesCountAsync(userId);
            return Json(new { success = true, message = "Favorilerden çıkarıldı.", favoriteCount });
        }

        // Bu metod GET isteği alabilir, UI'da sayfa yüklenirken direkt çağrılabilir.
        [HttpGet]
        public async Task<IActionResult> IsProductInFavorites(int productId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Json(new { isInFavorites = false });
            }
            int userId = int.Parse(userIdClaim);
            bool isInFavorites = await _favoriteService.IsProductInFavoritesAsync(userId, productId);
            return Json(new { isInFavorites });
        }
        
        // Add action metodu, AddToFavorites'a yönlendiriyor (hem GET hem de POST için çalışacak)
        public async Task<IActionResult> Add(int productId)
        {
            // return await AddToFavorites(productId); // Eski satır
            
            // Yeni implementasyon: JSON yanıtı yerine sayfa yönlendirmesi
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToAction("SignIn", "Account");
            }
            
            if (int.TryParse(userIdClaim, out int userId))
            {
                var product = await _productService.GetAsync(p => p.Id == productId);
                if (product == null)
                {
                    TempData["Error"] = "Ürün bulunamadı.";
                    return RedirectToAction("Index", "Home");
                }

                await _favoriteService.AddToFavoritesAsync(userId, productId);
                TempData["Success"] = "Ürün favorilerinize eklendi.";
            }
            
            return RedirectToAction("Index", "Favorites");
        }
    }
}
