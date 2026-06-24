using HeTHongDanhGiaThuoc.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeTHongDanhGiaThuoc.Controllers
{
    // =========================
    // CHỈ CHO PHÉP NGƯỜI DÙNG ĐÃ ĐĂNG NHẬP
    // =========================
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        // =========================
        // KHỞI TẠO CONTROLLER
        // =========================
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // DASHBOARD - TRANG CHỦ HỆ THỐNG
        // =========================
        public async Task<IActionResult> Index()
        {
            // =========================
            // THỐNG KÊ TỔNG QUAN HỆ THỐNG
            // =========================
            ViewBag.TongBenhNhan = await _context.HoSoBenhNhans.CountAsync();
            ViewBag.TongThuoc = await _context.Thuocs.CountAsync();
            ViewBag.TongDanhGia = await _context.DanhGias.CountAsync();
            ViewBag.TongNguoiDung = await _context.NguoiDungs.CountAsync();

            // =========================
            // LẤY DANH SÁCH ĐÁNH GIÁ GẦN ĐÂY (LATEST REVIEWS)
            // =========================
            var danhGiaGanDay = await _context.DanhGias
                .Include(d => d.HoSoBenhNhan)   // thông tin bệnh nhân
                .Include(d => d.NguoiDanhGia)   // người thực hiện đánh giá
                .OrderByDescending(d => d.NgayDanhGia) // mới nhất lên đầu
                .Take(5) // giới hạn 5 bản ghi gần nhất
                .ToListAsync();

            // Gửi sang View để hiển thị dashboard
            ViewBag.DanhGiaGanDay = danhGiaGanDay;

            // =========================
            // TÍNH TỶ LỆ PHÙ HỢP TRUNG BÌNH
            // =========================
            var tbPhuHop = await _context.DanhGias
                .Where(d => d.TyLePhuHop.HasValue)
                .AverageAsync(d => (double?)d.TyLePhuHop) ?? 0;

            // Làm tròn 1 chữ số thập phân
            ViewBag.TrungBinhPhuHop = Math.Round(tbPhuHop, 1);

            // =========================
            // TRẢ VỀ VIEW DASHBOARD
            // =========================
            return View();
        }

        // =========================
        // ERROR PAGE
        // =========================
        public IActionResult Error() => View();
    }
}