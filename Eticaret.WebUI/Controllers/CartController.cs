using Eticaret.Core.Entities;
using Eticaret.Service.Abstract;
using Eticaret.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Eticaret.Core.Enums;

namespace Eticaret.WebUI.Controllers
{
    public class CartController : Controller
    {
        private readonly IService<Product> _serviceProduct;
        private readonly ICartService _cartService;
        private readonly IService<AppUser> _serviceUser;
        private readonly IOrderService _orderService;
        private readonly IService<OrderItem> _serviceOrderItem;

        public CartController(IService<Product> serviceProduct, ICartService cartService, IService<AppUser> serviceUser, IOrderService orderService, IService<OrderItem> serviceOrderItem)
        {
            _serviceProduct = serviceProduct;
            _cartService = cartService;
            _serviceUser = serviceUser;
            _orderService = orderService;
            _serviceOrderItem = serviceOrderItem;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(userIdClaim))
                {
                    int userId = int.Parse(userIdClaim);
                    var cartItems = await _cartService.GetCartsByUserIdAsync(userId);
                    
                    var model = new CartViewModel()
                    {
                        CartLines = cartItems.Select(c => new CartLine 
                        { 
                            Id = c.Id,
                            Product = c.Product,
                            Quantity = c.Quantity
                        }).ToList(),
                        
                        TotalPrice = cartItems.Sum(c => c.Product?.Price * c.Quantity ?? 0)
                    };
                    
                    return View(model);
                }
            }
            
            return View(new CartViewModel { CartLines = new List<CartLine>(), TotalPrice = 0 });
        }

        [Authorize]
        public async Task<IActionResult> Add(int ProductId, int quantity = 1)
        {
            var product = _serviceProduct.Find(ProductId);
            if (product != null)
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(userIdClaim))
                {
                    int userId = int.Parse(userIdClaim);
                    await _cartService.AddToCartAsync(userId, ProductId, quantity);
                    
                    // AJAX isteği ise JSON yanıtı döndür
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        var cartItems = await _cartService.GetCartsByUserIdAsync(userId);
                        int cartCount = cartItems.Count();
                        return Json(new { success = true, message = "Ürün sepetinize eklendi.", cartCount });
                    }
                }
            }
            return RedirectToAction("Index");
        }

        [Authorize]
        public async Task<IActionResult> Update(int ProductId, int quantity = 1)
        {
            if (quantity > 0)
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(userIdClaim))
                {
                    int userId = int.Parse(userIdClaim);
                    
                    // Önce sepetten ürünü çıkarıp, sonra yeni miktarla ekleyelim
                    await _cartService.RemoveFromCartAsync(userId, ProductId);
                    await _cartService.AddToCartAsync(userId, ProductId, quantity);
                }
            }
            return RedirectToAction("Index");
        }

        [Authorize]
        public async Task<IActionResult> Remove(int ProductId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userIdClaim))
            {
                int userId = int.Parse(userIdClaim);
                await _cartService.RemoveFromCartAsync(userId, ProductId);
            }
            return RedirectToAction("Index");
        }

        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToAction("SignIn", "Account");
            }

            int userId = int.Parse(userIdClaim);
            var cartItems = await _cartService.GetCartsByUserIdAsync(userId);
            
            if (cartItems == null || !cartItems.Any())
            {
                return RedirectToAction("Index");
            }

            var user = await _serviceUser.GetAsync(u => u.Id == userId);
            
            var checkoutViewModel = new CheckoutViewModel
            {
                CartLines = cartItems.Select(c => new CartLine 
                { 
                    Id = c.Id,
                    Product = c.Product,
                    Quantity = c.Quantity
                }).ToList(),
                
                TotalPrice = cartItems.Sum(c => c.Product?.Price * c.Quantity ?? 0),
                ShippingCost = (cartItems.Sum(c => c.Product?.Price * c.Quantity ?? 0) > 500) ? 0 : 50
            };

            // Kullanıcı bilgilerini doldur
            if (user != null)
            {
                string[] nameParts = (user.Name + " " + user.Surname).Trim().Split(' ', 2);
                checkoutViewModel.FirstName = nameParts[0];
                checkoutViewModel.LastName = nameParts.Length > 1 ? nameParts[1] : "";
                checkoutViewModel.Email = user.Email;
                checkoutViewModel.Phone = user.Phone;
                checkoutViewModel.Address = user.Address;
            }

            return View(checkoutViewModel);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kullanıcı ID'si alınıyor
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int userId))
                {
                    // Sepetteki ürünleri al
                    var cartItems = await _cartService.GetCartsByUserIdAsync(userId);
                    
                    if (cartItems != null && cartItems.Any())
                    {
                        // Tek bir sipariş oluştur
                        var order = new Order
                        {
                            AppUserId = userId,
                            OrderDate = DateTime.Now,
                            Status = OrderStatus.SiparisAlindi,
                            OrderNumber = $"ORD-{DateTime.Now:yyyyMMddHHmmss}-{userId}",
                            TotalAmount = cartItems.Sum(c => c.Product?.Price * c.Quantity ?? 0)
                        };
                        
                        // Siparişi kaydet
                        await _orderService.AddAsync(order);
                        await _orderService.SaveChangesAsync();
                        
                        // Her ürün için sipariş detayı oluştur
                        foreach (var cartItem in cartItems)
                        {
                            var orderItem = new OrderItem
                            {
                                OrderId = order.Id,
                                ProductId = cartItem.ProductId,
                                Quantity = cartItem.Quantity,
                                UnitPrice = cartItem.Product?.Price ?? 0
                            };
                            
                            // Sipariş detayını kaydet
                            await _serviceOrderItem.AddAsync(orderItem);
                        }
                        
                        // Değişiklikleri veritabanına kaydet
                        await _serviceOrderItem.SaveChangesAsync();
                        
                        // Sepeti temizle
                        foreach (var cartItem in cartItems)
                        {
                            await _cartService.RemoveFromCartAsync(userId, cartItem.ProductId);
                        }
                        
                        // Başarılı ödeme mesajı ve sipariş tamamlandı sayfasına yönlendirme
                        TempData["Success"] = "Siparişiniz başarıyla oluşturuldu!";
                        return RedirectToAction("OrderComplete");
                    }
                }
            }

            // Form geçerli değilse, mevcut sepet bilgilerini tekrar doldurup sayfayı göster
            var userIdClaimInvalid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userIdClaimInvalid))
            {
                int userIdInvalid = int.Parse(userIdClaimInvalid);
                var cartItems = await _cartService.GetCartsByUserIdAsync(userIdInvalid);
                
                model.CartLines = cartItems.Select(c => new CartLine 
                { 
                    Id = c.Id,
                    Product = c.Product,
                    Quantity = c.Quantity
                }).ToList();
                
                model.TotalPrice = cartItems.Sum(c => c.Product?.Price * c.Quantity ?? 0);
                model.ShippingCost = (model.TotalPrice > 500) ? 0 : 50;
            }
            
            return View(model);
        }

        [Authorize]
        public IActionResult OrderComplete()
        {
            return View();
        }
    }
}
