using HeTHongDanhGiaThuoc.Data;
using HeTHongDanhGiaThuoc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeTHongDanhGiaThuoc.Controllers
{
    [Authorize]
    public class ThuocController : Controller
    {
        //Khởi tạo các contrustor
        private readonly ApplicationDbContext _context;

        public ThuocController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // INDEX - DANH SÁCH THUỐC
        // =========================
        public async Task<IActionResult> Index(string? search, int page = 1)
        {
            // =========================
            // KÍCH THƯỚC TRANG
            // =========================
            int pageSize = 10;

            // =========================
            // TRUY VẤN DỮ LIỆU THUỐC
            // =========================
            var query = _context.Thuocs
                // =========================
                // HOẠT CHẤT LIÊN KẾT (ĐANG DÙNG TRONG VIEW)
                // =========================
                .Include(t => t.ThuocHoatChats)
                    .ThenInclude(x => x.HoatChat)

                .AsQueryable();

            // =========================
            // TÌM KIẾM
            // =========================
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(t =>
                    t.TenThuoc.Contains(search) ||
                    (t.TenBietDuoc != null && t.TenBietDuoc.Contains(search)) ||
                    (t.TinhTrangThuoc != null && t.TinhTrangThuoc.Contains(search))
                );
            }

            // =========================
            // ĐẾM TỔNG SỐ BẢN GHI
            // =========================
            int total = await query.CountAsync();

            // =========================
            // LẤY DỮ LIỆU THEO TRANG
            // =========================
            var data = await query
                .OrderBy(t => t.TenThuoc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // =========================
            // VIEWBAG PHÂN TRANG
            // =========================
            ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.Search = search;

            return View(data);
        }

        // =========================
        // DETAILS - XEM CHI TIẾT THUỐC
        // =========================
        public async Task<IActionResult> Details(int id)
        {
            // =========================
            // TRUY VẤN THUỐC THEO ID
            // =========================
            var thuoc = await _context.Thuocs

                // =========================
                // LẤY DANH SÁCH HOẠT CHẤT
                // =========================
                .Include(t => t.ThuocHoatChats)
                    .ThenInclude(x => x.HoatChat)

                // =========================
                // LẤY CHỐNG CHỈ ĐỊNH (theo bệnh nền)
                // =========================
                .Include(t => t.ChongChiDinhs)
                    .ThenInclude(x => x.BenhNen)

                // =========================
                // LẤY TƯƠNG TÁC THUỐC (CHIỀU A)
                // =========================
                .Include(t => t.TuongTacA)
                    .ThenInclude(x => x.ThuocB)

                // =========================
                // LẤY TƯƠNG TÁC THUỐC (CHIỀU B)
                // =========================
                .Include(t => t.TuongTacB)
                    .ThenInclude(x => x.ThuocA)

                // =========================
                // LẤY THUỐC THEO ID
                // =========================
                .FirstOrDefaultAsync(t => t.MaThuoc == id);

            // =========================
            // KIỂM TRA TỒN TẠI
            // =========================
            if (thuoc == null)
                return NotFound();
            // Nếu không tìm thấy thuốc → trả về 404

            // =========================
            // TRẢ DỮ LIỆU RA VIEW
            // =========================
            return View(thuoc);
        }

        // =========================
        // CREATE GET
        // =========================
       
        [HttpGet]
        public async Task<IActionResult> Create()
        {
<<<<<<< HEAD
            ViewBag.HoatChats = await _context.HoatChats.ToListAsync();
            ViewBag.BenhNens = await _context.BenhNens.ToListAsync();
=======
            ViewBag.HoatChats = await _context.HoatChats.OrderBy(h => h.TenHoatChat).ToListAsync();
>>>>>>> origin/develop
            return View();
        }

        // =========================
        // CREATE POST
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            Thuoc model,
            List<int>? maHoatChats,
            List<string>? hamLuongs,
            List<int>? maBenhNens,
            List<string>? mucDoNguyHiem)
        {
            // =========================
            // LOẠI BỎ VALIDATION NAVIGATION (EF CORE MODEL BINDING)
            // =========================
            ModelState.Remove("ThuocHoatChats");
            ModelState.Remove("ChongChiDinhs");

            // =========================
            // KIỂM TRA VALIDATION TỔNG QUÁT
            // =========================
            if (!ModelState.IsValid)
            {
                // Nếu dữ liệu không hợp lệ → load lại dropdown
                ViewBag.HoatChats = await _context.HoatChats.ToListAsync();
                ViewBag.BenhNens = await _context.BenhNens.ToListAsync();

                return View(model);
            }

            // =========================
            // VALIDATE LOGIC NGHIỆP VỤ (Y KHOA)
            // =========================

            // Kiểm tra tuổi hợp lý
            if (model.TuoiToiThieu.HasValue && model.TuoiToiDa.HasValue)
            {
                if (model.TuoiToiThieu > model.TuoiToiDa)
                {
                    ModelState.AddModelError("", "Tuổi tối thiểu không được lớn hơn tuổi tối đa");
                    return View(model);
                }
            }

            // Kiểm tra cân nặng hợp lý
            if (model.CanNangToiThieu.HasValue && model.CanNangToiDa.HasValue)
            {
                if (model.CanNangToiThieu > model.CanNangToiDa)
                {
                    ModelState.AddModelError("", "Cân nặng không hợp lệ");
                    return View(model);
                }
            }

            // =========================
            // LƯU THUỐC CHÍNH
            // =========================
            _context.Thuocs.Add(model);
            await _context.SaveChangesAsync();
            // Sau bước này → model.MaThuoc đã được sinh ra

            // =========================
            // LƯU HOẠT CHẤT (MANY-TO-MANY)
            // =========================
            if (maHoatChats != null)
            {
                for (int i = 0; i < maHoatChats.Count; i++)//Duyệt từng hoạt chất trong ds hoạt chất
                {
                    if (maHoatChats[i] > 0) //Nếu hợp lệ
                    {
                        _context.ThuocHoatChats.Add(new ThuocHoatChat //thêm hoạt chất = hoạt chất của thuốc
                        {
                            MaThuoc = model.MaThuoc,
                            MaHoatChat = maHoatChats[i],

                            // Lấy hàm lượng tương ứng theo index (nếu có)
                            HamLuong = hamLuongs != null && i < hamLuongs.Count
                                ? hamLuongs[i]
                                : null
                        });
                    }
                }
            }

            // =========================
            // LƯU CHỐNG CHỈ ĐỊNH
            // =========================
            if (maBenhNens != null)
            {
                for (int i = 0; i < maBenhNens.Count; i++)//Duyệt từng bệnh nền trong ds bệnh nền
                {
                    if (maBenhNens[i] > 0)//Nếu hợp lệ
                    {
                        _context.ChongChiDinhs.Add(new ChongChiDinh //Thêm bệnh nền = Chống chỉ định (bệnh nền)
                        {
                            MaThuoc = model.MaThuoc,
                            MaBenhNen = maBenhNens[i],

                            // Mức độ nguy hiểm theo từng bệnh nền
                            MucDoNguyHiem = mucDoNguyHiem != null && i < mucDoNguyHiem.Count
                                ? mucDoNguyHiem[i]
                                : null
                        });
                    }
                }
            }

            // =========================
            // COMMIT DATABASE
            // =========================
            await _context.SaveChangesAsync();

            // =========================
            // THÔNG BÁO THÀNH CÔNG
            // =========================
            TempData["Success"] = "Thêm thuốc thành công!";

            // =========================
            // CHUYỂN SANG DETAILS
            // =========================
            return RedirectToAction(nameof(Details), new { id = model.MaThuoc });
        }

        // =========================
        // EDIT GET - CHỈNH SỬA THUỐC
        // =========================
        public async Task<IActionResult> Edit(int id)
        {
            // =========================
            // LẤY THUỐC THEO ID
            // =========================
            var thuoc = await _context.Thuocs
                // =========================
                // HOẠT CHẤT LIÊN KẾT
                // =========================
                .Include(t => t.ThuocHoatChats)

                // =========================
                // CHỐNG CHỈ ĐỊNH + BỆNH NỀN
                // =========================
                .Include(t => t.ChongChiDinhs)
                    .ThenInclude(c => c.BenhNen)

                // =========================
                // TƯƠNG TÁC THUỐC (ĐẦY ĐỦ 2 CHIỀU)
                // =========================
                .Include(t => t.TuongTacA)
                    .ThenInclude(x => x.ThuocB)

                .Include(t => t.TuongTacB)
                    .ThenInclude(x => x.ThuocA)

                // =========================
                // LẤY CHI TIẾT ĐÁNH GIÁ (nếu cần hiển thị edit nâng cao)
                // =========================
                .Include(t => t.ChiTietDanhGias)

                .FirstOrDefaultAsync(t => t.MaThuoc == id);

            // =========================
            // KIỂM TRA TỒN TẠI
            // =========================
            if (thuoc == null)
                return NotFound();

            // =========================
            // LOAD DANH MỤC DROPDOWN
            // =========================
            ViewBag.HoatChats = await _context.HoatChats.ToListAsync();
            ViewBag.BenhNens = await _context.BenhNens.ToListAsync();

            // ⚠️ QUAN TRỌNG: danh sách thuốc để tạo tương tác
            ViewBag.Thuocs = await _context.Thuocs.ToListAsync();

            // =========================
            // TRẢ VIEW
            // =========================
            return View(thuoc);
        }
        // =========================
        // EDIT POST - CẬP NHẬT THUỐC
        // =========================
  
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Thuoc model,
            List<int>? maHoatChats,
            List<string>? hamLuongs,
            List<int>? maBenhNens,
            List<string>? mucDoNguyHiem)
        {
            // =========================
            // KIỂM TRA ID TRÁNH GIẢ MẠO DỮ LIỆU
            // =========================
            if (id != model.MaThuoc)
                return BadRequest();

            // =========================
            // LOẠI BỎ VALIDATION NAVIGATION PROPERTY
            // (tránh ModelState lỗi do EF tracking)
            // =========================
            ModelState.Remove("ThuocHoatChats");
            ModelState.Remove("ChongChiDinhs");

            // =========================
            // KIỂM TRA VALIDATION FORM
            // =========================
            if (!ModelState.IsValid)
            {
                ViewBag.HoatChats = await _context.HoatChats.ToListAsync();
                ViewBag.BenhNens = await _context.BenhNens.ToListAsync();
                return View(model);
            }

            // =========================
            // LẤY THUỐC HIỆN CÓ TRONG DATABASE
            // =========================
            var thuoc = await _context.Thuocs.FindAsync(id);

            // =========================
            // KIỂM TRA TỒN TẠI
            // =========================
            if (thuoc == null)
                return NotFound();

            // =========================
            // CẬP NHẬT THÔNG TIN CƠ BẢN
            // =========================
            thuoc.TenThuoc = model.TenThuoc;
            thuoc.TenBietDuoc = model.TenBietDuoc;
            thuoc.DangBaoChe = model.DangBaoChe;
            thuoc.LieuDungChuan = model.LieuDungChuan;
            thuoc.TinhTrangThuoc = model.TinhTrangThuoc;
            thuoc.MoTaChiTiet = model.MoTaChiTiet;

            // =========================
            // GIỚI HẠN ĐỐI TƯỢNG SỬ DỤNG
            // =========================
            thuoc.TuoiToiThieu = model.TuoiToiThieu;
            thuoc.TuoiToiDa = model.TuoiToiDa;
            thuoc.CanNangToiThieu = model.CanNangToiThieu;
            thuoc.CanNangToiDa = model.CanNangToiDa;

            // =========================
            // LIỀU DÙNG CHUYÊN SÂU
            // =========================
            thuoc.LieuToiThieuMg = model.LieuToiThieuMg;
            thuoc.LieuToiDaMg = model.LieuToiDaMg;
            thuoc.LieuKhuyenNghiMg = model.LieuKhuyenNghiMg;
            thuoc.TanSuatToiDaMoiNgay = model.TanSuatToiDaMoiNgay;

            // =========================================================
            // HOẠT CHẤT (N-N)
            // - XÓA TOÀN BỘ CŨ
            // - THÊM LẠI THEO DỮ LIỆU MỚI
            // =========================================================
            var oldHC = _context.ThuocHoatChats
                .Where(x => x.MaThuoc == id);

            _context.ThuocHoatChats.RemoveRange(oldHC);

            if (maHoatChats != null)
            {
                for (int i = 0; i < maHoatChats.Count; i++)//Duyệt từng hoạt chất từ ds hoạt chất
                {
                    if (maHoatChats[i] > 0)
                    {
                        _context.ThuocHoatChats.Add(new ThuocHoatChat//Thêm hoạt chất = Hoạt chát của thuốc
                        {
                            MaThuoc = id,
                            MaHoatChat = maHoatChats[i],
                            HamLuong = (hamLuongs != null && i < hamLuongs.Count)
                                ? hamLuongs[i]
                                : null
                        });
                    }
                }
            }

            // =========================================================
            // CHỐNG CHỈ ĐỊNH (N-N)
            // - XÓA TOÀN BỘ CŨ
            // - THÊM LẠI THEO INPUT MỚI
            // =========================================================
            var oldCCD = _context.ChongChiDinhs
                .Where(x => x.MaThuoc == id);

            _context.ChongChiDinhs.RemoveRange(oldCCD);

            if (maBenhNens != null)
            {
                for (int i = 0; i < maBenhNens.Count; i++)//Duyệt từng bệnh nền từ ds bệnh nền
                {
                    if (maBenhNens[i] > 0)
                    {
                        _context.ChongChiDinhs.Add(new ChongChiDinh //Thêm bệnh nền = Chống chỉ định (bệnh nền )
                        {
                            MaThuoc = id,
                            MaBenhNen = maBenhNens[i],
                            MucDoNguyHiem = (mucDoNguyHiem != null && i < mucDoNguyHiem.Count)
                                ? mucDoNguyHiem[i]
                                : null
                        });
                    }
                }
            }

            // =========================
            // LƯU TOÀN BỘ THAY ĐỔI
            // =========================
            await _context.SaveChangesAsync();

            // =========================
            // THÔNG BÁO THÀNH CÔNG
            // =========================
            TempData["Success"] = "Cập nhật thuốc thành công!";

            // =========================
            // CHUYỂN SANG TRANG CHI TIẾT
            // =========================
            return RedirectToAction(nameof(Details), new { id });
        }

        // =========================
        // DELETE - XÓA THUỐC
        // =========================
        [Authorize(Roles = "Admin")]
        // Chỉ Admin mới có quyền xóa thuốc

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Bảo vệ chống CSRF (tránh request giả mạo từ bên ngoài)

        public async Task<IActionResult> Delete(int id)
        {
            // =========================
            // TÌM THUỐC THEO ID
            // =========================
            var thuoc = await _context.Thuocs.FindAsync(id);

            // Nếu không tồn tại thuốc → trả về 404
            if (thuoc == null)
                return NotFound();

            // =========================
            // XÓA DỮ LIỆU
            // =========================
            _context.Thuocs.Remove(thuoc);

            // Lưu thay đổi xuống database
            await _context.SaveChangesAsync();

            // =========================
            // THÔNG BÁO KẾT QUẢ
            // =========================
            TempData["Success"] = "Đã xóa thuốc thành công!";

            // =========================
            // CHUYỂN HƯỚNG VỀ DANH SÁCH
            // =========================
            return RedirectToAction(nameof(Index));
        }
    }
}