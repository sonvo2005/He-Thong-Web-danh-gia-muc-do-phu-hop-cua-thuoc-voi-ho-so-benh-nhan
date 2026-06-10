using HeTHongDanhGiaThuoc.Data;
using HeTHongDanhGiaThuoc.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HeTHongDanhGiaThuoc.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid) return View(model);

            var nguoiDung = await _context.NguoiDungs
                .Include(n => n.VaiTro)
                .FirstOrDefaultAsync(n => n.TenDangNhap == model.TenDangNhap && n.TrangThaiHoatDong);

            if (nguoiDung == null || !BCrypt.Net.BCrypt.Verify(model.MatKhau, nguoiDung.MatKhauMaHoa))
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, nguoiDung.MaNguoiDung.ToString()),
                new Claim(ClaimTypes.Name, nguoiDung.TenDangNhap),
                new Claim(ClaimTypes.GivenName, nguoiDung.HoTen),
                new Claim(ClaimTypes.Role, nguoiDung.VaiTro?.TenVaiTro ?? ""),
                new Claim("MaVaiTro", nguoiDung.MaVaiTro.ToString())
            };

            var identity = new ClaimsIdentity(claims, "CookieAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("CookieAuth", principal, new AuthenticationProperties
            {
                IsPersistent = model.GhiNho,
                ExpiresUtc = model.GhiNho ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.UtcNow.AddHours(8)
            });

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied() => View();
    }
}
