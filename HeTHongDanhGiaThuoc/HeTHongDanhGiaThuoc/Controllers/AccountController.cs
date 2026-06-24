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

        // =========================
        // KHỞI TẠO CONTROLLER
        // =========================
        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // LOGIN (GET) - HIỂN THỊ FORM ĐĂNG NHẬP
        // =========================
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            // Nếu người dùng đã đăng nhập rồi → không cho vào trang login nữa
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            // Lưu lại URL trước đó (nếu có) để sau khi login quay lại
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        // =========================
        // LOGIN (POST) - XỬ LÝ ĐĂNG NHẬP
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            // =========================
            // VALIDATE INPUT
            // =========================
            if (!ModelState.IsValid)
                return View(model);

            // =========================
            // KIỂM TRA NGƯỜI DÙNG TRONG DATABASE
            // =========================
            var nguoiDung = await _context.NguoiDungs
                .Include(n => n.VaiTro)
                .FirstOrDefaultAsync(n =>
                    n.TenDangNhap == model.TenDangNhap &&
                    n.TrangThaiHoatDong);

            // =========================
            // XÁC THỰC MẬT KHẨU
            // =========================
            if (nguoiDung == null ||
                !BCrypt.Net.BCrypt.Verify(model.MatKhau, nguoiDung.MatKhauMaHoa))
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng");
                return View(model);
            }

            // =========================
            // TẠO CLAIMS (THÔNG TIN NGƯỜI DÙNG)
            // =========================
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, nguoiDung.MaNguoiDung.ToString()),
                new Claim(ClaimTypes.Name, nguoiDung.TenDangNhap),
                new Claim(ClaimTypes.GivenName, nguoiDung.HoTen),
                new Claim(ClaimTypes.Role, nguoiDung.VaiTro?.TenVaiTro ?? ""),
                new Claim("MaVaiTro", nguoiDung.MaVaiTro.ToString())
            };

            // =========================
            // TẠO IDENTITY & PRINCIPAL
            // =========================
            var identity = new ClaimsIdentity(claims, "CookieAuth");
            var principal = new ClaimsPrincipal(identity);

            // =========================
            // ĐĂNG NHẬP BẰNG COOKIE AUTH
            // =========================
            await HttpContext.SignInAsync("CookieAuth", principal, new AuthenticationProperties
            {
                // Ghi nhớ đăng nhập nếu người dùng chọn
                IsPersistent = model.GhiNho,

                // Thời gian hết hạn cookie
                ExpiresUtc = model.GhiNho
                    ? DateTimeOffset.UtcNow.AddDays(7)
                    : DateTimeOffset.UtcNow.AddHours(8)
            });

            // =========================
            // CHUYỂN HƯỚNG SAU ĐĂNG NHẬP
            // =========================
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        // =========================
        // LOGOUT - ĐĂNG XUẤT
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Xóa cookie đăng nhập
            await HttpContext.SignOutAsync("CookieAuth");

            // Quay về trang login
            return RedirectToAction("Login");
        }

        // =========================
        // ACCESS DENIED - KHÔNG CÓ QUYỀN TRUY CẬP
        // =========================
        public IActionResult AccessDenied() => View();
    }
}