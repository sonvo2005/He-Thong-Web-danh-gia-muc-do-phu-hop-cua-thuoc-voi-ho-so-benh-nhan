using HeTHongDanhGiaThuoc.Data;
using HeTHongDanhGiaThuoc.Models;
using HeTHongDanhGiaThuoc.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeTHongDanhGiaThuoc.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===== NGƯỜI DÙNG =====
        public async Task<IActionResult> NguoiDung()
        {
            var list = await _context.NguoiDungs
                .Include(n => n.VaiTro)
                .OrderBy(n => n.HoTen)
                .ToListAsync();
            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> TaoNguoiDung()
        {
            ViewBag.VaiTros = await _context.VaiTros.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TaoNguoiDung(NguoiDungViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.VaiTros = await _context.VaiTros.ToListAsync();
                return View(model);
            }

            if (await _context.NguoiDungs.AnyAsync(n => n.TenDangNhap == model.TenDangNhap))
            {
                ModelState.AddModelError("TenDangNhap", "Tên đăng nhập đã tồn tại");
                ViewBag.VaiTros = await _context.VaiTros.ToListAsync();
                return View(model);
            }

            if (string.IsNullOrEmpty(model.MatKhauMoi))
            {
                ModelState.AddModelError("MatKhauMoi", "Mật khẩu là bắt buộc khi tạo mới");
                ViewBag.VaiTros = await _context.VaiTros.ToListAsync();
                return View(model);
            }

            var nguoiDung = new NguoiDung
            {
                TenDangNhap = model.TenDangNhap,
                MatKhauMaHoa = BCrypt.Net.BCrypt.HashPassword(model.MatKhauMoi),
                HoTen = model.HoTen,
                Email = model.Email,
                SoDienThoai = model.SoDienThoai,
                MaVaiTro = model.MaVaiTro,
                TrangThaiHoatDong = model.TrangThaiHoatDong,
                NgayTao = DateTime.Now
            };

            _context.NguoiDungs.Add(nguoiDung);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Tạo người dùng thành công!";
            return RedirectToAction(nameof(NguoiDung));
        }

        [HttpGet]
        public async Task<IActionResult> SuaNguoiDung(int id)
        {
            var n = await _context.NguoiDungs.FindAsync(id);
            if (n == null) return NotFound();

            var vm = new NguoiDungViewModel
            {
                MaNguoiDung = n.MaNguoiDung,
                TenDangNhap = n.TenDangNhap,
                HoTen = n.HoTen,
                Email = n.Email,
                SoDienThoai = n.SoDienThoai,
                MaVaiTro = n.MaVaiTro,
                TrangThaiHoatDong = n.TrangThaiHoatDong
            };

            ViewBag.VaiTros = await _context.VaiTros.ToListAsync();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuaNguoiDung(NguoiDungViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.VaiTros = await _context.VaiTros.ToListAsync();
                return View(model);
            }

            var n = await _context.NguoiDungs.FindAsync(model.MaNguoiDung);
            if (n == null) return NotFound();

            n.HoTen = model.HoTen;
            n.Email = model.Email;
            n.SoDienThoai = model.SoDienThoai;
            n.MaVaiTro = model.MaVaiTro;
            n.TrangThaiHoatDong = model.TrangThaiHoatDong;

            if (!string.IsNullOrEmpty(model.MatKhauMoi))
                n.MatKhauMaHoa = BCrypt.Net.BCrypt.HashPassword(model.MatKhauMoi);

            await _context.SaveChangesAsync();
            TempData["Success"] = "Cập nhật người dùng thành công!";
            return RedirectToAction(nameof(NguoiDung));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XoaNguoiDung(int id)
        {
            var n = await _context.NguoiDungs.FindAsync(id);
            if (n == null) return NotFound();
            _context.NguoiDungs.Remove(n);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã xóa người dùng.";
            return RedirectToAction(nameof(NguoiDung));
        }

        // ===== TƯƠNG TÁC THUỐC =====
        public async Task<IActionResult> TuongTacThuoc()
        {
            var list = await _context.TuongTacThuocs
                .Include(t => t.ThuocA)
                .Include(t => t.ThuocB)
                .OrderBy(t => t.MucDoNghiemTrong)
                .ToListAsync();
            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> ThemTuongTac()
        {
            ViewBag.Thuocs = await _context.Thuocs.OrderBy(t => t.TenThuoc).ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemTuongTac(TuongTacThuoc model)
        {
            ModelState.Remove("ThuocA");
            ModelState.Remove("ThuocB");

            if (model.MaThuoc_A == model.MaThuoc_B)
            {
                ModelState.AddModelError("", "Không thể chọn cùng một thuốc");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Thuocs = await _context.Thuocs.OrderBy(t => t.TenThuoc).ToListAsync();
                return View(model);
            }

            _context.TuongTacThuocs.Add(model);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Thêm tương tác thuốc thành công!";
            return RedirectToAction(nameof(TuongTacThuoc));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XoaTuongTac(int id)
        {
            var tt = await _context.TuongTacThuocs.FindAsync(id);
            if (tt == null) return NotFound();
            _context.TuongTacThuocs.Remove(tt);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã xóa tương tác thuốc.";
            return RedirectToAction(nameof(TuongTacThuoc));
        }

        // ===== HOẠT CHẤT =====
        public async Task<IActionResult> HoatChat()
        {
            var list = await _context.HoatChats.OrderBy(h => h.TenHoatChat).ToListAsync();
            return View(list);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemHoatChat(string tenHoatChat)
        {
            if (!string.IsNullOrWhiteSpace(tenHoatChat))
            {
                _context.HoatChats.Add(new HoatChat { TenHoatChat = tenHoatChat });
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm hoạt chất thành công!";
            }
            return RedirectToAction(nameof(HoatChat));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XoaHoatChat(int id)
        {
            var hc = await _context.HoatChats.FindAsync(id);
            if (hc == null) return NotFound();
            _context.HoatChats.Remove(hc);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã xóa hoạt chất.";
            return RedirectToAction(nameof(HoatChat));
        }
    }
}
