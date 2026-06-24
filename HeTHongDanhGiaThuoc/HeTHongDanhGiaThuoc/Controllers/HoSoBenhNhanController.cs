using HeTHongDanhGiaThuoc.Data;
using HeTHongDanhGiaThuoc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Diagnostics;
namespace HeTHongDanhGiaThuoc.Controllers
{
    [Authorize]
    public class HoSoBenhNhanController : Controller
    {
        //Khởi tạo constructor
        private readonly ApplicationDbContext _context; //Khai báo database

        public HoSoBenhNhanController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ================= LIST (Danh Sách Hồ Sơ Bệnh Nhân) =================
        //Nhiệm vụ:
        //1. Lấy danh sách hồ sơ bệnh nhân
        // 2. Tìm kiếm theo tên hoặc mã định danh
        // 3.Phân trang
        public async Task<IActionResult> Index(string? search, int page = 1)
        {
            int pageSize = 10; //1 trang hiển thị tối đa 10 bệnh nhân

            var query = _context.HoSoBenhNhans //Truy cập database để tải dữ liệu HoSoBenhNhan
                .Include(x => x.NguoiTao)//Lấy luôn thông tin người tạo
                .AsQueryable();// Cho phép gắn thêm điều kiện động phía dưới

            // Nếu người dùng nhập từ khóa tìm kiếm
            if (!string.IsNullOrWhiteSpace(search))
            {
                // Lọc theo họ tên hoặc mã định danh bệnh nhân
                query = query.Where(x =>
                    x.HoTen.Contains(search) ||
                    x.MaDinhDanhBenhNhan.Contains(search));

                // Trả lại từ khóa để hiển thị trên ô tìm kiếm
                ViewBag.Search = search;
            }
            // Đếm tổng số bản ghi sau khi đã lọc
            int total = await query.CountAsync();

            // Lấy dữ liệu theo trang
            var data = await query
                .OrderByDescending(x => x.NgayTiepNhan) // Đẩy hồ sơ mới nhất lên đầu
                .Skip((page - 1) * pageSize)            // Bỏ qua các bản ghi của trang trước
                .Take(pageSize)                         // Chỉ lấy số bản ghi của 1 trang
                .ToListAsync();

            // Tính tổng số trang
            ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);

            // Trang hiện tại
            ViewBag.CurrentPage = page;

            // Tổng số hồ sơ tìm được
            ViewBag.Total = total;

            return View(data);
        }

        // ================= DETAILS =================
        public async Task<IActionResult> Details(int id)
        {
            var data = await _context.HoSoBenhNhans //Truy vấn database
            //Lấy các thông tin của hồ sơ bệnh nhân
                .Include(x => x.NguoiTao)  //Người tạo hồ sơ
                .Include(x => x.BenhNhanBenhNens) //Danh sách bệnh nền
                    .ThenInclude(x => x.BenhNen)

                .Include(x => x.DiUngThuocs) //Danh sách dị ứng (dị ứng với những hoạt chất nào)
                    .ThenInclude(x => x.HoatChat)

                .Include(x => x.ThuocDangSuDungs)//Danh Sách thuốc đang sử dụng
                     .ThenInclude(x => x.Thuoc)

                .Include(x => x.DanhGias) //Danh sách đánh giá
                    .ThenInclude(x => x.NguoiDanhGia)//Người đánh giá

                .Include(x => x.DanhGias)//Danh sách đánh giá
                    .ThenInclude(x => x.ChiTietDanhGias)//Danh sách chi tiết đánh giá
                        .ThenInclude(x => x.Thuoc)

                .FirstOrDefaultAsync(x => x.MaBenhNhan == id); //Lấy hồ sơ bệnh nhân theo mã bệnh nhân


            if (data == null) return NotFound();
            return View(data);
        }

        // ================= CREATE =================
        [HttpGet]//Get là phương tức lấy dữ liệu
        public async Task<IActionResult> Create() //Gọi hàm khi người dùng ấn "Thêm bệnh nhân"
        {
            ViewBag.HoatChats = await _context.HoatChats //Tải dữ liệu hoạt chất từ database
                .OrderBy(x => x.TenHoatChat) //Sắp xếp theo tên hoạt chất
                .ToListAsync();

            ViewBag.BenhNens = await _context.BenhNens //Tải dữ liệu bệnh nền từ database
                .OrderBy(x => x.TenBenhNen) //Sắp xếp theo tên bệnh nền
                .ToListAsync();

            ViewBag.Thuocs = await _context.Thuocs//Tải dữ liệu thuốc (đang sử dụng) từ database
                .OrderBy(x => x.TenThuoc)//Sắp xếp theo tên thuốc
                .ToListAsync();

            // var dsThuoc = await _context.Thuocs
            // .OrderBy(x => x.TenThuoc)
            // .ToListAsync();

            // ViewBag.Thuocs = dsThuoc;

            // ViewBag.DebugThuocCount = dsThuoc.Count;
            return View();
        }

        [HttpPost] //Post là phương thức tải dữ liệu về cho server từ views
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( //Gọi hàm khi người dùng ấn "Lưu" sau khi đã nhập đầu đủ thông tin
            HoSoBenhNhan model, //Truyền vàp model Hosobenhnhan
                                //=> Chứa các dữ liệu sẽ tải lên như sao
            List<string>? tenBenhNens,
            List<DateTime>? ngayChanDoans,
            List<int>? maHoatChats,
            List<string>? mucDoDiUngs,
            List<int>? maThuocs,
            List<string>? lieuDungs,
            List<DateTime>? ngayBatDaus)
        {
            //Đồng thời xóa các trường dữ liệu ko cần thiết
            ModelState.Remove(nameof(model.NguoiTao));
            ModelState.Remove(nameof(model.BenhNhanBenhNens));
            ModelState.Remove(nameof(model.DiUngThuocs));
            ModelState.Remove(nameof(model.DanhGias));

            //Kiểm tra dữ liệu có hợp lệ
            if (!ModelState.IsValid)
                return View(model); //nếu trống thì ko cho gửi

            bool exist = await _context.HoSoBenhNhans //truy vấn database 
                .AnyAsync(x => x.MaDinhDanhBenhNhan == model.MaDinhDanhBenhNhan); //kiểm tra "Mã định danh" của bệnh nhân

            if (exist) //nếu trùng thì xuất thông báo
            {
                ModelState.AddModelError("", "Mã định danh đã tồn tại");
                return View(model);
            }

            //Gán ID của người tạo hồ sơ bệnh nhân
            model.MaNguoiTao = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            model.NgayTiepNhan = DateTime.Now;//Ghi nhận ngày tạo

            //Lưu hồ sơ bệnh nhân vào database
            _context.HoSoBenhNhans.Add(model);
            await _context.SaveChangesAsync();

            // ===== BỆNH NỀN =====
            if (tenBenhNens != null)
            {
                for (int i = 0; i < tenBenhNens.Count; i++)//Duyệt từng bệnh nền từ ds bệnh nền
                {
                    if (!string.IsNullOrWhiteSpace(tenBenhNens[i]))//Bỏ qua dòng rỗng
                    {
                        var benhNen = await _context.BenhNens //truy vấn database xem bệnh nền đã tồn tại chưa
                            .FirstOrDefaultAsync(x => x.TenBenhNen == tenBenhNens[i]);

                        if (benhNen == null) //Nếu rỗng
                        {
                            benhNen = new BenhNen
                            {
                                TenBenhNen = tenBenhNens[i] //Thêm bệnh nền đó là bệnh nền mới
                            };

                            _context.BenhNens.Add(benhNen); //Lưu vào database
                            await _context.SaveChangesAsync();
                        }

                        _context.BenhNhanBenhNens.Add(new BenhNhanBenhNen //Là bảng trung gian chứa bệnh nhân và bệnh nền 
                        {
                            MaBenhNhan = model.MaBenhNhan,
                            MaBenhNen = benhNen.MaBenhNen,
                            NgayChanDoan =
                                (ngayChanDoans != null && i < ngayChanDoans.Count)
                                    ? ngayChanDoans[i]
                                    : DateTime.Now
                        });
                    }
                }
            }

            // ===== DỊ ỨNG =====
            if (maHoatChats != null)
            {
                for (int i = 0; i < maHoatChats.Count; i++)//Duyệt từng hoạt chất từ ds hoạt chất
                {
                    if (maHoatChats[i] > 0) //Nếu hợp lệ
                    {
                        _context.DiUngThuocs.Add(new DiUngThuoc //Thêm hoạt chất đó = dị ứng của bệnh nhân 
                        {
                            MaBenhNhan = model.MaBenhNhan,//Mã bệnh nhân
                            MaHoatChat = maHoatChats[i],//Mã hoạt chất
                            MucDoDiUng = (mucDoDiUngs != null && i < mucDoDiUngs.Count)//Mức độ dị ứng
                                ? mucDoDiUngs[i]
                                : ""
                        });
                    }
                }
            }

            // ===== THUỐC ĐANG SỬ DỤNG =====
            if (maThuocs != null)
            {
                for (int i = 0; i < maThuocs.Count; i++)//Duyệt từng thuốc từ ds thuốc
                {
                    if (maThuocs[i] > 0) //Nếu hợp lệ
                    {
                        _context.ThuocDangSuDungs.Add(//Tạo thuốc đang sử dụng = Thuốc từ ds thuốc
                            new ThuocDangSuDung
                            {
                                MaBenhNhan = model.MaBenhNhan,

                                MaThuoc = maThuocs[i],

                                LieuDung =
                                    (lieuDungs != null &&
                                     i < lieuDungs.Count)
                                        ? lieuDungs[i]
                                        : "",

                                NgayBatDau =
                                    (ngayBatDaus != null &&
                                     i < ngayBatDaus.Count)
                                        ? ngayBatDaus[i]
                                        : null
                            });
                    }
                }
            }
            await _context.SaveChangesAsync();//Lưu hồ sơ bệnh nhân vào database

            TempData["Success"] = "Tạo hồ sơ thành công"; //Thống báo tạo hồ sơ bệnh nhân thành công
            return RedirectToAction(nameof(Details), new { id = model.MaBenhNhan });
        }

        // ================= EDIT =================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var benhNhan = await _context.HoSoBenhNhans //Truy vấn database
                .Include(x => x.BenhNhanBenhNens)//Lấy ds bệnh nền theo bệnh nhân
                    .ThenInclude(x => x.BenhNen)//Lấy ds bệnh nền 
                .Include(x => x.DiUngThuocs) //Lấy ds dị ứng (dị ứng với những hoạt chất nào)
                .Include(x => x.ThuocDangSuDungs)
                .FirstOrDefaultAsync(x => x.MaBenhNhan == id);

            if (benhNhan == null)
                return NotFound();

            ViewBag.HoatChats = await _context.HoatChats//Tải dữ hoạt chất (dị ứng hoạt chất) từ database
                .OrderBy(x => x.TenHoatChat)//Sắp xếp theo tên hoạt chất
                .ToListAsync();

            ViewBag.BenhNens = await _context.BenhNens//Tải dữ liệu bệnh nền từ ds bệnh nền 
                .OrderBy(x => x.TenBenhNen)//Sắp xếp theo bệnh nền 
                .ToListAsync();


            ViewBag.Thuocs = await _context.Thuocs//Tải dữ liệu thuốc (đang sử dụng) từ database
             .OrderBy(x => x.TenThuoc)//Sắp xếp theo tên thuốc
             .ToListAsync();

            return View(benhNhan);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
        int id,
        HoSoBenhNhan model,
        List<string>? tenBenhNens,
        List<DateTime>? ngayChanDoans,
        List<int>? maHoatChats,
        List<string>? mucDoDiUngs,
        List<int>? maThuocs,
        List<string>? lieuDungs,
        List<DateTime>? ngayBatDaus)
        {
            // ===== KIỂM TRA ID =====
            if (id != model.MaBenhNhan)
                return BadRequest();

            // ===== BỎ QUA FIELD NAVIGATION KHÔNG VALIDATE =====
            //Xóa những dữ liệu ko cần thiết
            ModelState.Remove(nameof(model.NguoiTao));
            ModelState.Remove(nameof(model.BenhNhanBenhNens));
            ModelState.Remove(nameof(model.DiUngThuocs));
            ModelState.Remove(nameof(model.DanhGias));


            // ===== KIỂM TRA VALID =====

            if (!ModelState.IsValid)
            {
                foreach (var item in ModelState)
                {
                    foreach (var error in item.Value.Errors)
                    {
                        Debug.WriteLine($"{item.Key} => {error.ErrorMessage}");
                    }
                }

                return View(model);
            }
            if (!ModelState.IsValid)//Kiểm tra dữ liệu có hợp lệ
                return View(model);

            //Truy vấn HoSoBenhNhan Từ database
            var entity = await _context.HoSoBenhNhans
                .FirstOrDefaultAsync(x => x.MaBenhNhan == id);

            if (entity == null)
                return NotFound();

            // =========================
            // UPDATE THÔNG TIN CƠ BẢN
            // =========================
            entity.HoTen = model.HoTen;
            entity.MaDinhDanhBenhNhan = model.MaDinhDanhBenhNhan;
            entity.GioiTinh = model.GioiTinh;
            entity.NgaySinh = model.NgaySinh;
            entity.DiaChi = model.DiaChi;

            //  BỔ SUNG:
            entity.NhomMau = model.NhomMau;
            entity.ChieuCao = model.ChieuCao;
            entity.CanNang = model.CanNang;
            entity.BenhHienTai = model.BenhHienTai;
            entity.GhiChu = model.GhiChu;

            // =========================
            // XÓA DỮ LIỆU CŨ
            // =========================
            TempData["Debug"] =
                $"maThuocs={(maThuocs == null ? "NULL" : string.Join(",", maThuocs))}";
            _context.BenhNhanBenhNens.RemoveRange(
                _context.BenhNhanBenhNens.Where(x => x.MaBenhNhan == id));

            _context.DiUngThuocs.RemoveRange(
                _context.DiUngThuocs.Where(x => x.MaBenhNhan == id));

            _context.ThuocDangSuDungs.RemoveRange(
                _context.ThuocDangSuDungs.Where(x => x.MaBenhNhan == id));

            // =========================
            // THÊM BỆNH NỀN MỚI
            // =========================
            if (tenBenhNens != null)
            {
                for (int i = 0; i < tenBenhNens.Count; i++)//Duyệt từng bệnh nền trong ds bệnh nền 
                {
                    if (!string.IsNullOrWhiteSpace(tenBenhNens[i])) //truy vấn database xem bệnh nền đã tồn tại chưa
                    {
                        var benhNen = await _context.BenhNens
                            .FirstOrDefaultAsync(x => x.TenBenhNen == tenBenhNens[i]);

                        if (benhNen == null)//Nếu bệnh nền ko có 
                        {
                            benhNen = new BenhNen//Thêm bệnh nền mới = bệnh nền đó
                            {
                                TenBenhNen = tenBenhNens[i]
                            };

                            _context.BenhNens.Add(benhNen);
                            await _context.SaveChangesAsync(); // Lưu lại bệnh nền 
                        }

                        _context.BenhNhanBenhNens.Add(new BenhNhanBenhNen //Là bảng trung gian chứa bệnh nhân và bệnh nền 
                        {
                            MaBenhNhan = id,
                            MaBenhNen = benhNen.MaBenhNen,
                            NgayChanDoan =
                                (ngayChanDoans != null && i < ngayChanDoans.Count)
                                    ? ngayChanDoans[i]
                                    : DateTime.Now
                        });
                    }
                }
            }

            // =========================
            // THÊM DỊ ỨNG
            // =========================
            if (maHoatChats != null)
            {
                for (int i = 0; i < maHoatChats.Count; i++)//Duyệt từng hoạt chất trong ds hoạt chất
                {
                    if (maHoatChats[i] > 0) //nếu hợp lệ
                    {
                        _context.DiUngThuocs.Add(new DiUngThuoc //Thêm Hoạt chất đó = dị ứng thuốc của bệnh nhân 
                        {
                            MaBenhNhan = id,
                            MaHoatChat = maHoatChats[i],
                            MucDoDiUng =
                                (mucDoDiUngs != null && i < mucDoDiUngs.Count)
                                    ? mucDoDiUngs[i]
                                    : ""
                        });
                    }
                }
            }

            // =========================
            // THÊM THUỐC ĐANG DÙNG
            // =========================
            if (maThuocs != null)
            {
                for (int i = 0; i < maThuocs.Count; i++)//Duyệt từng thuốc trong ds thuốc
                {
                    if (maThuocs[i] > 0) //nếu hợp lệ
                    {
                        _context.ThuocDangSuDungs.Add(new ThuocDangSuDung//Thêm thuốc đó = Thuốc đang sử dụng của bệnh nhân
                        {
                            MaBenhNhan = id,
                            MaThuoc = maThuocs[i],
                            LieuDung =
                                (lieuDungs != null && i < lieuDungs.Count)
                                    ? lieuDungs[i]
                                    : null,
                            NgayBatDau =
                                (ngayBatDaus != null && i < ngayBatDaus.Count)
                                    ? ngayBatDaus[i]
                                    : null
                        });
                    }
                }
            }

            // ===== LƯU DATABASE =====
            await _context.SaveChangesAsync();

            ViewBag.HoatChats = await _context.HoatChats.ToListAsync();
            ViewBag.BenhNens = await _context.BenhNens.ToListAsync();
            ViewBag.Thuocs = await _context.Thuocs.ToListAsync();

            TempData["Success"] = "Cập nhật thành công";

            return RedirectToAction(nameof(Details), new { id });
        }

        // ================= DELETE =================
        [HttpPost] // Chỉ nhận request POST (an toàn hơn GET khi xóa dữ liệu)
        [ValidateAntiForgeryToken] // Chống tấn công CSRF
        [Authorize(Roles = "Admin,BacSi,DuocSi")] // Chỉ Admin hoặc Bác sĩ, Dược Sĩ mới được quyền xóa
        public async Task<IActionResult> Delete(int id) // Nhận ID bệnh nhân cần xóa
        {
            // ================= LẤY BỆNH NHÂN =================
            var entity = await _context.HoSoBenhNhans //Truy vấn database
                .FirstOrDefaultAsync(x => x.MaBenhNhan == id); // Tìm bệnh nhân theo ID

            // ================= KIỂM TRA TỒN TẠI =================
            if (entity == null) // Nếu không tìm thấy
                return NotFound(); // Trả về 404

            // ================= XÓA DỮ LIỆU LIÊN QUAN (QUAN TRỌNG) =================

            // Xóa danh sách bệnh nền của bệnh nhân
            var benhNens = _context.BenhNhanBenhNens
                .Where(x => x.MaBenhNhan == id);

            _context.BenhNhanBenhNens.RemoveRange(benhNens);

            // Xóa danh sách dị ứng thuốc
            var diUngs = _context.DiUngThuocs
                .Where(x => x.MaBenhNhan == id);

            _context.DiUngThuocs.RemoveRange(diUngs);

            // Xóa danh sách thuốc đang sử dụng
            var thuocs = _context.ThuocDangSuDungs
                .Where(x => x.MaBenhNhan == id);

            _context.ThuocDangSuDungs.RemoveRange(thuocs);

            // ================= XÓA BỆNH NHÂN =================
            _context.HoSoBenhNhans.Remove(entity); // Xóa hồ sơ bệnh nhân chính

            // ================= LƯU DATABASE =================
            await _context.SaveChangesAsync(); // Commit toàn bộ thay đổi

            // ================= THÔNG BÁO =================
            TempData["Success"] = "Đã xóa hồ sơ"; // Gửi thông báo sang View

            // ================= CHUYỂN TRANG =================
            return RedirectToAction(nameof(Index)); // Quay về danh sách
        }
    }
}