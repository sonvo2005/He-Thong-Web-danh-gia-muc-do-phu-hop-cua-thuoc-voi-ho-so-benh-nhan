using HeTHongDanhGiaThuoc.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeTHongDanhGiaThuoc.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TongBenhNhan = await _context.HoSoBenhNhans.CountAsync();
            ViewBag.TongThuoc = await _context.Thuocs.CountAsync();
            ViewBag.TongDanhGia = await _context.DanhGias.CountAsync();
            ViewBag.TongNguoiDung = await _context.NguoiDungs.CountAsync();

            // Đánh giá gần đây
            var danhGiaGanDay = await _context.DanhGias
                .Include(d => d.HoSoBenhNhan)
                .Include(d => d.NguoiDanhGia)
                .OrderByDescending(d => d.NgayDanhGia)
                .Take(5)
                .ToListAsync();

            ViewBag.DanhGiaGanDay = danhGiaGanDay;

            // Thống kê tỷ lệ phù hợp trung bình
            var tbPhuHop = await _context.DanhGias
                .Where(d => d.TyLePhuHop.HasValue)
                .AverageAsync(d => (double?)d.TyLePhuHop) ?? 0;
            ViewBag.TrungBinhPhuHop = Math.Round(tbPhuHop, 1);

            return View();
        }

        public IActionResult Error() => View();
    }
}
