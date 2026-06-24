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
        //Khởi tạo constructor
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        //================ NGƯỜI DÙNG ================
        // ===== NGƯỜI DÙNG(Trang hiển thị danh sách người dùng) =====
        public async Task<IActionResult> NguoiDung()
        {
            var list = await _context.NguoiDungs  //Truy vấn và lấy ds toàn bộ người dùng
                .Include(n => n.VaiTro)
                .OrderBy(n => n.HoTen)
                .ToListAsync();
            return View(list);
        }

        // ===== CREATE Người Dùng =====
        [HttpGet]
        public async Task<IActionResult> TaoNguoiDung()
        {
            ViewBag.VaiTros = await _context.VaiTros.ToListAsync();//Tải dữ liệu các vai trò từ database
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TaoNguoiDung(NguoiDungViewModel model)
        {
            if (!ModelState.IsValid)//Nếu lỗi dữ liệu => quay lại trang 
            {
                ViewBag.VaiTros = await _context.VaiTros.ToListAsync();
                return View(model);
            }

            if (await _context.NguoiDungs.AnyAsync(n => n.TenDangNhap == model.TenDangNhap)) //Nếu tên đăng nhập đã có
            {
                ModelState.AddModelError("TenDangNhap", "Tên đăng nhập đã tồn tại");
                ViewBag.VaiTros = await _context.VaiTros.ToListAsync();
                return View(model);
            }

            if (string.IsNullOrEmpty(model.MatKhauMoi))//Nếu mật khẩu trùng với mk cũ
            {
                ModelState.AddModelError("MatKhauMoi", "Mật khẩu là bắt buộc khi tạo mới");
                ViewBag.VaiTros = await _context.VaiTros.ToListAsync();
                return View(model);
            }

            var nguoiDung = new NguoiDung//Tiến hành tạo người dùng mới
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

            _context.NguoiDungs.Add(nguoiDung); //Thêm vào bảngh Người Dùng ở Database

            await _context.SaveChangesAsync();

            TempData["Success"] = "Tạo người dùng thành công!";
            
            return RedirectToAction(nameof(NguoiDung));
        }


        // ===== EDIT Người Dùng =====
        [HttpGet]
        public async Task<IActionResult> SuaNguoiDung(int id)
        {
            var n = await _context.NguoiDungs.FindAsync(id); //Tìm người dùng theo id

            if (n == null) 
                return NotFound();//Nếu ko tìm thấy thì trả 404

            var vm = new NguoiDungViewModel //Tải dữ liệu của người dùng theo ID đó
            {
                MaNguoiDung = n.MaNguoiDung,
                TenDangNhap = n.TenDangNhap,
                HoTen = n.HoTen,
                Email = n.Email,
                SoDienThoai = n.SoDienThoai,
                MaVaiTro = n.MaVaiTro,
                TrangThaiHoatDong = n.TrangThaiHoatDong
            };

            ViewBag.VaiTros = await _context.VaiTros.ToListAsync();//Tải dữ liệu các vai trò từ database
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuaNguoiDung(NguoiDungViewModel model)
        {
            if (!ModelState.IsValid)//Nếu lỗi dữ liệu => quay lại trang
            {
                ViewBag.VaiTros = await _context.VaiTros.ToListAsync();//Tải dữ liệu các vai trò từ database
                return View(model);
            }

            var n = await _context.NguoiDungs.FindAsync(model.MaNguoiDung);//Tìm người dùng theo id

            if (n == null) 
                return NotFound();//Nếu ko tìm thấy thì trả 404

            //Cập nhật lại các thông tin
            n.HoTen = model.HoTen;
            n.Email = model.Email;
            n.SoDienThoai = model.SoDienThoai;
            n.MaVaiTro = model.MaVaiTro;
            n.TrangThaiHoatDong = model.TrangThaiHoatDong;

            if (!string.IsNullOrEmpty(model.MatKhauMoi))//Nếu mật khẩu mới khác với mật khẻ cũ
                n.MatKhauMaHoa = BCrypt.Net.BCrypt.HashPassword(model.MatKhauMoi);//Câp nhật mật khẩu mới

            await _context.SaveChangesAsync();//Lưu databse

            TempData["Success"] = "Cập nhật người dùng thành công!";
            return RedirectToAction(nameof(NguoiDung));
        }

    // ===== DELETE Người Dùng =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XoaNguoiDung(int id)
        {
            var n = await _context.NguoiDungs.FindAsync(id);//Lấy người dùng theo ID

            if (n == null) //Nếu ko tìm thấy thì trả 404
                return NotFound();

            _context.NguoiDungs.Remove(n);//Xóa dữ liệu người dùng

            await _context.SaveChangesAsync();//Cập nhật lại database
            TempData["Success"] = "Đã xóa người dùng.";

            return RedirectToAction(nameof(NguoiDung));
        }

        // ===== TƯƠNG TÁC THUỐC =====

        // ===== Trang hiển thị danh sách tương tác thuốc =====
        public async Task<IActionResult> TuongTacThuoc()
        {
            var list = await _context.TuongTacThuocs //Truy vấn database và tải dữ liệu
                .Include(t => t.ThuocA)
                .Include(t => t.ThuocB)
                .OrderBy(t => t.MucDoNghiemTrong)
                .ToListAsync();
            return View(list);
        }

    // ===== CREATE tương tác thuốc =====
        [HttpGet]
        public async Task<IActionResult> ThemTuongTac()
        {
            ViewBag.Thuocs = await _context.Thuocs.OrderBy(t => t.TenThuoc).ToListAsync();//Tải dữ liệu các thuốc từ database
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemTuongTac(TuongTacThuoc model)
        {
            ModelState.Remove("ThuocA");
            ModelState.Remove("ThuocB");

            if (model.MaThuocA == model.MaThuocB)
            {
                ModelState.AddModelError("", "Không thể chọn cùng một thuốc");
            }

            if (!ModelState.IsValid)//Nếu dữ liệu không hợp lệ
            {
                ViewBag.Thuocs = await _context.Thuocs.OrderBy(t => t.TenThuoc).ToListAsync();
                return View(model);//Quay lai trang
            }

            _context.TuongTacThuocs.Add(model);//Thêm vào database

            await _context.SaveChangesAsync();//Lưu database
            TempData["Success"] = "Thêm tương tác thuốc thành công!";
            return RedirectToAction(nameof(TuongTacThuoc));
        }

        // ===== DELETE tương tác thuốc =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XoaTuongTac(int id)
        {
            var tt = await _context.TuongTacThuocs.FindAsync(id);//Lấy tương tác thuốc theo ID

            if (tt == null) //Nếu ko tìm thấy thì trả 404
                return NotFound();

            _context.TuongTacThuocs.Remove(tt);//Xóa khỏi database

            await _context.SaveChangesAsync();//Cập nhật lại datababse

            TempData["Success"] = "Đã xóa tương tác thuốc.";
            return RedirectToAction(nameof(TuongTacThuoc));
        }

        // ===== HOẠT CHẤT =====
        // ===== Trang hiển thị danh sách hoạt chất =====
        public async Task<IActionResult> HoatChat()
        {
            var list = await _context.HoatChats
            .Include(h => h.ThuocHoatChats)
            .OrderBy(h => h.TenHoatChat)
            .ToListAsync();//Tải dữ liệu các hoạt chất từ database
            return View(list);
        }
        
        // ===== CREATE hoạt chất =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemHoatChat(string tenHoatChat)
        {
            if (!string.IsNullOrWhiteSpace(tenHoatChat))//Nếu tên hoạt chất khác rỗng
            {
                _context.HoatChats.Add(new HoatChat { TenHoatChat = tenHoatChat });//Thêm hoạt chất
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm hoạt chất thành công!";
            }
            return RedirectToAction(nameof(HoatChat));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XoaHoatChat(int id)
        {
            var hc = await _context.HoatChats.FindAsync(id);//Lấy hoạt chất theo ID

            if (hc == null) //Nếu ko tìm thấy thì trả 404
                return NotFound();

            _context.HoatChats.Remove(hc);//Xóa khỏi database

            await _context.SaveChangesAsync();//Cập nhật databse
            TempData["Success"] = "Đã xóa hoạt chất.";
            return RedirectToAction(nameof(HoatChat));
        }


        // ===== BỆNH NỀN =====
        // ===== Trang hiển thị danh sách bệnh nền =====
        public async Task<IActionResult> BenhNen()
        {
            var list = await _context.BenhNens //Truy vấn database và tải dữ liệu
                .OrderBy(b => b.TenBenhNen)
                .ToListAsync();

            return View(list);
        }

        // ===== CREATE bệnh nền =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemBenhNen(string tenBenhNen)
        {
            if (!string.IsNullOrWhiteSpace(tenBenhNen))//Nếu tên bệnh nền ko rỗng
            {
                _context.BenhNens.Add(new BenhNen //Thêm bệnh nền 
                {
                    TenBenhNen = tenBenhNen
                });

                await _context.SaveChangesAsync();

                TempData["Success"] = "Thêm bệnh nền thành công!";
            }

            return RedirectToAction(nameof(BenhNen));
        }

        // ===== DELETE bệnh nền =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XoaBenhNen(int id)
        {
            var bn = await _context.BenhNens.FindAsync(id);//Lấy bệnh nền theo ID

            if (bn == null)//Nếu ko tìm thấy thì trả 404
                return NotFound();

            _context.BenhNens.Remove(bn);//Xóa khỏi databse

            await _context.SaveChangesAsync();//Cập nhật database

            TempData["Success"] = "Đã xóa bệnh nền.";

            return RedirectToAction(nameof(BenhNen));
        }

        
    }
    }