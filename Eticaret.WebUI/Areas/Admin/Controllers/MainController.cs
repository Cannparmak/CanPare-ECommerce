using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Eticaret.Core.Entities;
using Eticaret.Data;
using Microsoft.AspNetCore.Authorization;

namespace Eticaret.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminPolicy")]
    public class MainController : Controller
    {
        private readonly DatabaseContext _context;

        public MainController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: Admin/Main
        public async Task<IActionResult> Index(string search)
        {
            var users = from u in _context.AppUsers
                        select u;

            if (!string.IsNullOrEmpty(search))
            {
                users = users.Where(u => u.Name.Contains(search) ||
                                         u.Surname.Contains(search) ||
                                         u.Email.Contains(search));
                ViewData["CurrentFilter"] = search;
            }

            return View(await users.ToListAsync());
        }
    }
}
