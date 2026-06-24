using HeTHongDanhGiaThuoc.Services;
using HeTHongDanhGiaThuoc.Data;
using HeTHongDanhGiaThuoc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using HeTHongDanhGiaThuoc.Models.Requests;
namespace HeTHongDanhGiaThuoc.Controllers
{
    [Authorize]
    public class DanhGiaController : Controller
    {
        //Khởi tạo các constructor
        private readonly ApplicationDbContext _context;
        private readonly IDanhGiaService _danhGiaService; //Dùng để gọi tới Service

        public DanhGiaController(
            ApplicationDbContext context,
            IDanhGiaService danhGiaService)
        {
            _context = context;
            _danhGiaService = danhGiaService;
        }

        // =====================================================
        // INDEX - TRANG DANH SÁCH
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> Index(string? search, int page = 1)
        {
            int pageSize = 10; //Mỗi trang hiển thị tối đa 10 đánh giá

            var query = _context.DanhGias //Truy vấn dữ liệu đánh giá từ database
                .Include(d => d.HoSoBenhNhan)
                .Include(d => d.NguoiDanhGia)
                .Include(d => d.ChiTietDanhGias)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search)) //Nếu người dùng nhập từ khóa tìm kiếm
            {
                query = query.Where(x =>
                    x.HoSoBenhNhan!.HoTen.Contains(search) ||
                    x.HoSoBenhNhan.MaDinhDanhBenhNhan.Contains(search));
            }

            var total = await query.CountAsync();// Đếm tổng số bản ghi

            var items = await query //Lấy dữ liệu theo trang
                .OrderByDescending(x => x.NgayDanhGia)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            

            ViewBag.Search = search;//Trả lại từ khóa tìm kiếm
            ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.Total = total; // Thêm dòng này để hiển thị tổng

            return View(items);
        }

        // =====================================================
        // CREATE 
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> Create(int? maBenhNhan)
        {
            ViewBag.BenhNhans = await _context.HoSoBenhNhans// Lấy toàn bộ danh sách hồ sơ bệnh nhân
                .OrderBy(x => x.HoTen) // Sắp xếp theo tên A-Z giúp dễ tìm kiếm
                .ToListAsync();

            ViewBag.Thuocs = await _context.Thuocs    // Lấy danh sách thuốc đang còn hiệu lực sử dụng
                .Include(t => t.ThuocHoatChats) //Hiển thị hoạt chất của thuốc
                    .ThenInclude(th => th.HoatChat)

                .Include(t => t.ChongChiDinhs)
                     .ThenInclude(c => c.BenhNen) //Hiển thị chống chỉ định của thuốc


                .Include(t => t.TuongTacA)
                  .ThenInclude(x => x.ThuocB)
                .Include(t => t.TuongTacB)
                 .ThenInclude(x => x.ThuocA)

                .Where(t => t.TinhTrangThuoc == "Hoạt động")            // Chỉ cho phép đánh giá các thuốc đang hoạt động
                .OrderBy(t => t.TenThuoc) // Sắp xếp theo tên thuốc
                .ToListAsync();

           
            if (maBenhNhan.HasValue) // Nếu người dùng đi từ trang hồ sơ bệnh nhân
            // sang chức năng đánh giá và truyền sẵn mã bệnh nhân
            {
                
                ViewBag.BenhNhanChon = await _context.HoSoBenhNhans// Truy vấn database lấy dữ liệu

                    .Include(h => h.BenhNhanBenhNens)  // Load danh sách bệnh nền của bệnh nhân
                        .ThenInclude(x => x.BenhNen)

                    .Include(h => h.DiUngThuocs)// Load danh sách dị ứng thuốc của bệnh nhân
                        .ThenInclude(d => d.HoatChat)

                   
                    .Include(h => h.ThuocDangSuDungs) //Load danh sách Thuốc đang sử dụng
                        .ThenInclude(t => t.Thuoc)

                    // Tìm đúng bệnh nhân được truyền lên
                    .FirstOrDefaultAsync(h => h.MaBenhNhan == maBenhNhan);
            }
          
            // Trả về giao diện Create
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken] // Chống tấn công CSRF
        public async Task<IActionResult> Create(
            // Tiếp nhận dữ liệu từ form đánh giá
            // Sau đó chuyển sang DanhGiaService để xử lý nghiệp vụ
            int maBenhNhan,                 // Mã bệnh nhân được đánh giá
            List<int> maThuocs,             // Danh sách thuốc được chọn
            List<string?> lieuDungs,        // Liều mô tả (vd: 500mg x 2 lần/ngày)
            List<decimal?> lieuMgs,         // Liều mỗi lần (mg)
            List<int?> soLanMoiNgays,       // Số lần sử dụng trong ngày
            string? ghiChu)                 // Ghi chú cho phiên đánh giá
        {
            // Kiểm tra người dùng đã chọn thuốc hay chưa
            if (maThuocs == null || !maThuocs.Any())
            {
                TempData["Error"] = "Vui lòng chọn thuốc.";

                // Quay lại màn hình Create
                return RedirectToAction(nameof(Create), new { maBenhNhan });
            }

            // Lấy ID người dùng đang đăng nhập
            // ClaimTypes.NameIdentifier thường chứa khóa chính User
            var maNguoiDung = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // Gọi tầng Service để thực hiện:
            // - Kiểm tra dị ứng
            // - Kiểm tra chống chỉ định
            // - Kiểm tra tương tác thuốc
            // - Kiểm tra tuổi
            // - Kiểm tra cân nặng
            // - Kiểm tra liều dùng
            // - Tính điểm phù hợp
            // - Tạo DanhGia
            // - Tạo ChiTietDanhGia
            var request = new TaoDanhGiaRequest
            {
                MaBenhNhan = maBenhNhan,
                MaNguoiDung = maNguoiDung,
                GhiChu = ghiChu,
                Thuocs = maThuocs.Select((id, i) => new ThuocDanhGiaInput
                {
                    MaThuoc = id,
                    LieuMg = lieuMgs.ElementAtOrDefault(i),
                    SoLanMoiNgay = soLanMoiNgays.ElementAtOrDefault(i),
                    LieuDung = lieuDungs.ElementAtOrDefault(i)
                }).ToList()
            };

            var id = await _danhGiaService.TaoDanhGiaAsync(request);

            // Chuyển sang trang kết quả đánh giá
            return RedirectToAction(nameof(KetQua), new { id });
        }

        // =====================================================
        // KET QUA (DETAIL RESULT)
        // Hiển thị toàn bộ kết quả của một phiên đánh giá thuốc
        // Bao gồm:
        // - Thông tin bệnh nhân
        // - Bệnh nền
        // - Dị ứng hoạt chất
        // - Thuốc đang sử dụng
        // - Danh sách thuốc được đánh giá
        // - Kết quả đánh giá chi tiết
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> KetQua(int id)
        {
            // Lấy phiên đánh giá theo mã đánh giá
            var danhGia = await _context.DanhGias
            .Include(d => d.HoSoBenhNhan)
                .ThenInclude(h => h!.BenhNhanBenhNens)
                    .ThenInclude(bn => bn.BenhNen)

            .Include(d => d.HoSoBenhNhan)
                .ThenInclude(h => h!.DiUngThuocs)
                    .ThenInclude(du => du.HoatChat)

            .Include(d => d.HoSoBenhNhan)
                .ThenInclude(h => h!.ThuocDangSuDungs)
                    .ThenInclude(td => td.Thuoc)

            .Include(d => d.NguoiDanhGia)

            .Include(d => d.ChiTietDanhGias)
                .ThenInclude(ct => ct.Thuoc)
                    .ThenInclude(t => t!.ThuocHoatChats)
                        .ThenInclude(thc => thc.HoatChat)

            // CHỐNG CHỈ ĐỊNH
            .Include(d => d.ChiTietDanhGias)
                .ThenInclude(ct => ct.Thuoc)
                    .ThenInclude(t => t!.ChongChiDinhs)
                        .ThenInclude(cc => cc.BenhNen)

            // TƯƠNG TÁC A
            .Include(d => d.ChiTietDanhGias)
                .ThenInclude(ct => ct.Thuoc)
                    .ThenInclude(t => t!.TuongTacA)
                        .ThenInclude(tt => tt.ThuocB)

            // TƯƠNG TÁC B
            .Include(d => d.ChiTietDanhGias)
                .ThenInclude(ct => ct.Thuoc)
                    .ThenInclude(t => t!.TuongTacB)
                        .ThenInclude(tt => tt.ThuocA)

            .FirstOrDefaultAsync(d => d.MaDanhGia == id);

            // Không tìm thấy dữ liệu
            if (danhGia == null)
                return NotFound();

           

            // Trả dữ liệu sang View
            return View(danhGia);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LuuKetLuan(
    int maDanhGia,
    string? ketLuan)
        {
            var danhGia = await _context.DanhGias //Truy vấn database 
                .FirstOrDefaultAsync(x => x.MaDanhGia == maDanhGia); //tìm ID của đánh giá trong DB

            if (danhGia == null) //Nếu rỗng thì quay lại 
                return NotFound();

            danhGia.KetLuan = ketLuan; //Cập nhật lại phần Kết Luận

            await _context.SaveChangesAsync();//Lưu DB

            TempData["Success"] = "Đã lưu kết luận.";

            return RedirectToAction(
                nameof(KetQua),
                new { id = maDanhGia });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var danhGia = await _context.DanhGias //Truy vấn database 
                .Include(d => d.ChiTietDanhGias)
                .FirstOrDefaultAsync(d => d.MaDanhGia == id); //Chọn Id của đánh giá

            if (danhGia == null)//nếu rỗng thì quay lại
                return NotFound();

            if (danhGia.ChiTietDanhGias != null)
            {
                _context.ChiTietDanhGias.RemoveRange(danhGia.ChiTietDanhGias);//Xóa dữ liệu trong ChiTietDanhGias
            }

            _context.DanhGias.Remove(danhGia);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Xóa thành công.";

            return RedirectToAction(nameof(Index));
        }
    }
}