using Eticaret.Core.Entities;
using Eticaret.Service.Abstract;
using Eticaret.WebUI.Models;
using Microsoft.AspNetCore.Authentication; //Login Kütüphanesi
using Microsoft.AspNetCore.Authorization; //Login Kütüphanesi
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims; //Login Kütüphanesi

namespace Eticaret.WebUI.Controllers
{
    public class AccountController : Controller
    {
        //private readonly DatabaseContext _context;

        //public AccountController(DatabaseContext context)
        //{
        //    _context = context;
        //}
        private readonly IService<AppUser> _service;
        private readonly IOrderService _orderService;

        public AccountController(IService<AppUser> service, IOrderService orderService)
        {
            _service = service;
            _orderService = orderService;
        }

        [Authorize]
        public async Task<IActionResult> IndexAsync()
        {
            var userGuidClaim = HttpContext.User.FindFirst("UserGuid");
            if (userGuidClaim == null)
            {
                return RedirectToAction("SignIn");
            }
            
            AppUser user = await _service.GetAsync(x=>x.UserGuid.ToString() == userGuidClaim.Value);
            if(user is null)
            {
                return NotFound();
            }
            var model = new UserEditViewModel() 
            {
                Email = user.Email,
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Password = user.Password,
                Phone = user.Phone,
                UserName = user.UserName,
                Address = user.Address
            };
            return View(model);
        }
        
        [Authorize]
        public async Task<IActionResult> MyOrders()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return RedirectToAction("SignIn");
            }

            var orders = await _orderService.GetOrdersWithItemsByUserIdAsync(userId);
            return View(orders);
        }
        
        [HttpPost, Authorize]
        public async Task<IActionResult> IndexAsync(UserEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userGuidClaim = HttpContext.User.FindFirst("UserGuid");
                    if (userGuidClaim == null)
                    {
                        return RedirectToAction("SignIn");
                    }
                    
                    AppUser user = await _service.GetAsync(x => x.UserGuid.ToString() == userGuidClaim.Value);
                    if (user is not null)
                    {
                        user.Email = model.Email;
                        user.Name = model.Name;
                        user.Surname = model.Surname;
                        user.Password = model.Password;
                        user.Phone = model.Phone;
                        user.UserName = model.UserName;
                        user.Address = model.Address;
                        _service.Update(user);
                        var sonuc = _service.SaveChanges();
                        if (sonuc > 0)
                        {
                            TempData["Message"] = @"<div class=""alert alert-success alert-dismissible fade show"" 
                           role=""alert"">
                         <strong>Kullanıcı Başarıyla Güncellenmiştir!</strong>
  <button type=""button"" class=""btn-close"" data-bs-dismiss=""alert"" aria-label=""Close""></button>
</div>";
                            // await MailHelper.SendMailAsync(contact);
                            return RedirectToAction("Index");
                        }
                    }                
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Hata Oluştur");
                }
            }
            return View(model);
        }
        public IActionResult SignIn()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignInAsync(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var account = await _service.GetAsync(x=>x.Email == loginViewModel.Email & x.Password == loginViewModel.Password & x.IsActive);
                    if (account == null)
                    {
                        ModelState.AddModelError("", "Giriş Başarısız!");
                    }
                    else 
                    {
                        var claims = new List<Claim>() 
                        { 
                            new(ClaimTypes.Name, account.Name),
                            new(ClaimTypes.Role, account.IsAdmin ? "Admin" : "Customer"),
                            new(ClaimTypes.Email, account.Email),
                            new(ClaimTypes.NameIdentifier, account.Id.ToString()),
                            new("UserId", account.Id.ToString()),
                            new("UserGuid", account.UserGuid.ToString()),
                        };
                        var userIdentity = new ClaimsIdentity(claims, "Login");
                        ClaimsPrincipal userPrincipal = new ClaimsPrincipal(userIdentity);
                        await HttpContext.SignInAsync(userPrincipal);
                        return Redirect(string.IsNullOrEmpty(loginViewModel.ReturnUrl) ? "/" : loginViewModel.ReturnUrl);
                    }
                }
                catch (Exception)
                {
                    //Loglama Yaparken Kullanılacak
                    ModelState.AddModelError("", "Hata Oluştu!");
                }
            }
            return View();
        }
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignUpAsync(AppUser appUser)
        {
            // E-posta adresinin daha önce kullanılıp kullanılmadığını kontrol et
            var existingUser = await _service.GetAsync(x => x.Email == appUser.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Bu e-posta adresi zaten kullanılmaktadır.");
                return View(appUser);
            }

            appUser.IsAdmin = false;
            appUser.IsActive = true;
            if (ModelState.IsValid)
            {
                appUser.CreateDate = DateTime.Now;
                appUser.UserGuid = Guid.NewGuid();

                await _service.AddAsync(appUser);
                await _service.SaveChangesAsync();

                // Otomatik giriş yapma
                var claims = new List<Claim>() 
                { 
                    new(ClaimTypes.Name, appUser.Name),
                    new(ClaimTypes.Role, "Customer"),
                    new(ClaimTypes.Email, appUser.Email),
                    new(ClaimTypes.NameIdentifier, appUser.Id.ToString()),
                    new("UserId", appUser.Id.ToString()),
                    new("UserGuid", appUser.UserGuid.ToString()),
                };
                var userIdentity = new ClaimsIdentity(claims, "Login");
                ClaimsPrincipal userPrincipal = new ClaimsPrincipal(userIdentity);
                await HttpContext.SignInAsync(userPrincipal);

                return RedirectToAction("Index", "Home"); // Ana sayfaya yönlendir
            }

            return View(appUser);
        }
        public async Task<IActionResult> SignOutAsync()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("SignIn");
        }
    }
}
