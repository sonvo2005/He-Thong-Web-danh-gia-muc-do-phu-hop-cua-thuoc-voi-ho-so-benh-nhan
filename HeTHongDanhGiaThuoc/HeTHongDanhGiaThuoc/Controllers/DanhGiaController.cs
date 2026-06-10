using HeTHongDanhGiaThuoc.Data;
using HeTHongDanhGiaThuoc.Models;
using HeTHongDanhGiaThuoc.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HeTHongDanhGiaThuoc.Controllers
{
    [Authorize]
    public class DanhGiaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DanhGiaController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? search, int page = 1)
        {
            int pageSize = 10;
            var query = _context.DanhGias
                .Include(d => d.HoSoBenhNhan)
                .Include(d => d.NguoiDanhGia)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(d =>
                    d.HoSoBenhNhan!.HoTen.Contains(search) ||
                    d.HoSoBenhNhan.MaDinhDanhBenhNhan.Contains(search));
                ViewBag.Search = search;
            }

            int total = await query.CountAsync();
            var items = await query
                .OrderByDescending(d => d.NgayDanhGia)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.Total = total;

            return View(items);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? maBenhNhan)
        {
            ViewBag.BenhNhans = await _context.HoSoBenhNhans
                .OrderBy(h => h.HoTen).ToListAsync();
            ViewBag.Thuocs = await _context.Thuocs
                .Where(t => t.TinhTrangThuoc == "Hoạt động")
                .OrderBy(t => t.TenThuoc).ToListAsync();

            if (maBenhNhan.HasValue)
            {
                var bn = await _context.HoSoBenhNhans
                    .Include(h => h.BenhNens)
                    .Include(h => h.DiUngThuocs).ThenInclude(d => d.HoatChat)
                    .FirstOrDefaultAsync(h => h.MaBenhNhan == maBenhNhan);
                ViewBag.BenhNhanChon = bn;
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int maBenhNhan, List<int> maThuocs, List<string?> lieuDungs, string? ghiChu)
        {
            if (!maThuocs.Any() || maThuocs.All(m => m == 0))
            {
                TempData["Error"] = "Vui lòng chọn ít nhất một thuốc để đánh giá.";
                return RedirectToAction(nameof(Create), new { maBenhNhan });
            }

            var maNguoiDung = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // Load patient full info
            var benhNhan = await _context.HoSoBenhNhans
                .Include(h => h.BenhNens)
                .Include(h => h.DiUngThuocs).ThenInclude(d => d.HoatChat)
                .FirstOrDefaultAsync(h => h.MaBenhNhan == maBenhNhan);

            if (benhNhan == null)
            {
                TempData["Error"] = "Không tìm thấy hồ sơ bệnh nhân.";
                return RedirectToAction(nameof(Create));
            }

            // Load selected drugs with relations
            var thuocDachon = await _context.Thuocs
                .Include(t => t.ThuocHoatChats).ThenInclude(th => th.HoatChat)
                .Include(t => t.ChongChiDinhs)
                .Include(t => t.TuongTacA).ThenInclude(tt => tt.ThuocB)
                .Include(t => t.TuongTacB).ThenInclude(tt => tt.ThuocA)
                .Where(t => maThuocs.Contains(t.MaThuoc))
                .ToListAsync();

            // ===== EVALUATION LOGIC =====
            var chiTiets = new List<ChiTietDanhGia>();
            var canhBaos = new List<string>();
            int tongThuoc = thuocDachon.Count;
            int soThuocPhuHop = 0;

            var hoatChatDiUngIds = benhNhan.DiUngThuocs
                .Where(d => d.MaHoatChat.HasValue)
                .Select(d => d.MaHoatChat!.Value)
                .ToHashSet();

            var tenBenhNens = benhNhan.BenhNens.Select(b => b.TenBenhNen.ToLower()).ToList();

            for (int i = 0; i < thuocDachon.Count; i++)
            {
                var thuoc = thuocDachon[i];
                var lyDos = new List<string>();
                bool phuHop = true;

                // 1. Check dị ứng hoạt chất
                var hoatChatThuoc = thuoc.ThuocHoatChats.Select(th => th.MaHoatChat).ToHashSet();
                var diUngMatched = hoatChatThuoc.Intersect(hoatChatDiUngIds).ToList();
                if (diUngMatched.Any())
                {
                    phuHop = false;
                    var tenHoatChat = thuoc.ThuocHoatChats
                        .Where(th => diUngMatched.Contains(th.MaHoatChat))
                        .Select(th => th.HoatChat?.TenHoatChat ?? "")
                        .Where(s => !string.IsNullOrEmpty(s));
                    lyDos.Add($"Bệnh nhân dị ứng với hoạt chất: {string.Join(", ", tenHoatChat)}");
                    canhBaos.Add($"[{thuoc.TenThuoc}] DỊ ỨNG HOẠT CHẤT - {string.Join(", ", tenHoatChat)}");
                }

                // 2. Check chống chỉ định với bệnh nền
                foreach (var ccd in thuoc.ChongChiDinhs)
                {
                    var tenBenh = ccd.TenBenhChongChiDinh.ToLower();
                    if (tenBenhNens.Any(bn => bn.Contains(tenBenh) || tenBenh.Contains(bn)))
                    {
                        phuHop = false;
                        lyDos.Add($"Chống chỉ định với bệnh nền: {ccd.TenBenhChongChiDinh}");
                        canhBaos.Add($"[{thuoc.TenThuoc}] CHỐNG CHỈ ĐỊNH - {ccd.TenBenhChongChiDinh}");
                    }
                }

                // 3. Check tương tác thuốc trong danh sách
                foreach (var thuocKhac in thuocDachon)
                {
                    if (thuocKhac.MaThuoc == thuoc.MaThuoc) continue;
                    var tuongTac = thuoc.TuongTacA
                        .FirstOrDefault(tt => tt.MaThuoc_B == thuocKhac.MaThuoc)
                        ?? thuoc.TuongTacB.FirstOrDefault(tt => tt.MaThuoc_A == thuocKhac.MaThuoc);

                    if (tuongTac != null)
                    {
                        var severity = tuongTac.MucDoNghiemTrong?.ToLower() ?? "";
                        if (severity.Contains("nghiêm trọng") || severity.Contains("cao") || severity.Contains("nguy hiểm"))
                        {
                            phuHop = false;
                            lyDos.Add($"Tương tác nghiêm trọng với {thuocKhac.TenThuoc}: {tuongTac.MoTaTuongTac}");
                        }
                        else
                        {
                            lyDos.Add($"Tương tác ({tuongTac.MucDoNghiemTrong}) với {thuocKhac.TenThuoc}");
                        }
                        canhBaos.Add($"TƯƠNG TÁC [{thuoc.TenThuoc}] - [{thuocKhac.TenThuoc}]: {tuongTac.MoTaTuongTac}");
                    }
                }

                if (phuHop) soThuocPhuHop++;

                chiTiets.Add(new ChiTietDanhGia
                {
                    MaThuoc = thuoc.MaThuoc,
                    LieuDungChiDinh = lieuDungs != null && i < lieuDungs.Count ? lieuDungs[i] : null,
                    CoPhuHopKhong = phuHop,
                    LyDoKhongPhuHop = lyDos.Any() ? string.Join("; ", lyDos) : null
                });
            }

            decimal tyLePhuHop = tongThuoc > 0 ? Math.Round((decimal)soThuocPhuHop / tongThuoc * 100, 1) : 0;
            string khuyenNghi = tyLePhuHop >= 80
                ? "Phác đồ thuốc phù hợp. Có thể sử dụng với theo dõi thông thường."
                : tyLePhuHop >= 50
                    ? "Phác đồ thuốc cần xem xét lại. Một số thuốc có cảnh báo quan trọng."
                    : "Phác đồ thuốc KHÔNG phù hợp. Cần điều chỉnh ngay trước khi sử dụng.";

            var danhGia = new DanhGia
            {
                MaBenhNhan = maBenhNhan,
                MaNguoiDanhGia = maNguoiDung,
                NgayDanhGia = DateTime.Now,
                TyLePhuHop = tyLePhuHop,
                CanhBaoYTe = canhBaos.Any() ? string.Join("\n", canhBaos) : "Không có cảnh báo đặc biệt",
                KhuyenNghiSuDung = khuyenNghi,
                GhiChuPhienDanhGia = ghiChu,
                ChiTietDanhGias = chiTiets
            };

            _context.DanhGias.Add(danhGia);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đánh giá thành công!";
            return RedirectToAction(nameof(KetQua), new { id = danhGia.MaDanhGia });
        }

        public async Task<IActionResult> KetQua(int id)
        {
            var danhGia = await _context.DanhGias
                .Include(d => d.HoSoBenhNhan).ThenInclude(h => h!.BenhNens)
                .Include(d => d.HoSoBenhNhan).ThenInclude(h => h!.DiUngThuocs).ThenInclude(du => du.HoatChat)
                .Include(d => d.NguoiDanhGia)
                .Include(d => d.ChiTietDanhGias).ThenInclude(c => c.Thuoc)
                .FirstOrDefaultAsync(d => d.MaDanhGia == id);

            if (danhGia == null) return NotFound();
            return View(danhGia);
        }

        public async Task<IActionResult> Details(int id)
        {
            var danhGia = await _context.DanhGias
                .Include(d => d.HoSoBenhNhan)
                .Include(d => d.NguoiDanhGia)
                .Include(d => d.ChiTietDanhGias).ThenInclude(c => c.Thuoc)
                .FirstOrDefaultAsync(d => d.MaDanhGia == id);

            if (danhGia == null) return NotFound();
            return View(danhGia);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Bác sĩ")]
        public async Task<IActionResult> Delete(int id)
        {
            var danhGia = await _context.DanhGias.FindAsync(id);
            if (danhGia == null) return NotFound();
            _context.DanhGias.Remove(danhGia);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã xóa đánh giá.";
            return RedirectToAction(nameof(Index));
        }
    }
}
