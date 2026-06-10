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
        private readonly ApplicationDbContext _context;

        public ThuocController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? search, int page = 1)
        {
            int pageSize = 10;
            var query = _context.Thuocs
                .Include(t => t.ThuocHoatChats).ThenInclude(th => th.HoatChat)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(t =>
                    t.TenThuoc.Contains(search) ||
                    (t.TenBietDuoc != null && t.TenBietDuoc.Contains(search)));
                ViewBag.Search = search;
            }

            int total = await query.CountAsync();
            var items = await query
                .OrderBy(t => t.TenThuoc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.Total = total;

            return View(items);
        }

        public async Task<IActionResult> Details(int id)
        {
            var thuoc = await _context.Thuocs
                .Include(t => t.ThuocHoatChats).ThenInclude(th => th.HoatChat)
                .Include(t => t.ChongChiDinhs)
                .Include(t => t.TuongTacA).ThenInclude(tt => tt.ThuocB)
                .Include(t => t.TuongTacB).ThenInclude(tt => tt.ThuocA)
                .FirstOrDefaultAsync(t => t.MaThuoc == id);

            if (thuoc == null) return NotFound();
            return View(thuoc);
        }

        [Authorize(Roles = "Admin,Dược sĩ")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.HoatChats = await _context.HoatChats.OrderBy(h => h.TenHoatChat).ToListAsync();
            return View();
        }

        [Authorize(Roles = "Admin,Dược sĩ")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Thuoc model,
            List<int>? maHoatChats, List<string>? hamLuongs,
            List<string>? tenBenhChongChiDinhs, List<string>? ghiChuChongChiDinhs)
        {
            ModelState.Remove("ThuocHoatChats");
            ModelState.Remove("ChongChiDinhs");
            ModelState.Remove("TuongTacA");
            ModelState.Remove("TuongTacB");
            ModelState.Remove("ChiTietDanhGias");

            if (!ModelState.IsValid)
            {
                ViewBag.HoatChats = await _context.HoatChats.OrderBy(h => h.TenHoatChat).ToListAsync();
                return View(model);
            }

            _context.Thuocs.Add(model);
            await _context.SaveChangesAsync();

            if (maHoatChats != null)
            {
                for (int i = 0; i < maHoatChats.Count; i++)
                {
                    if (maHoatChats[i] > 0)
                    {
                        _context.ThuocHoatChats.Add(new ThuocHoatChat
                        {
                            MaThuoc = model.MaThuoc,
                            MaHoatChat = maHoatChats[i],
                            HamLuong = hamLuongs != null && i < hamLuongs.Count ? hamLuongs[i] : null
                        });
                    }
                }
            }

            if (tenBenhChongChiDinhs != null)
            {
                for (int i = 0; i < tenBenhChongChiDinhs.Count; i++)
                {
                    if (!string.IsNullOrWhiteSpace(tenBenhChongChiDinhs[i]))
                    {
                        _context.ChongChiDinhs.Add(new ChongChiDinh
                        {
                            MaThuoc = model.MaThuoc,
                            TenBenhChongChiDinh = tenBenhChongChiDinhs[i],
                            GhiChu = ghiChuChongChiDinhs != null && i < ghiChuChongChiDinhs.Count ? ghiChuChongChiDinhs[i] : null
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Thêm thuốc thành công!";
            return RedirectToAction(nameof(Details), new { id = model.MaThuoc });
        }

        [Authorize(Roles = "Admin,Dược sĩ")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var thuoc = await _context.Thuocs
                .Include(t => t.ThuocHoatChats).ThenInclude(th => th.HoatChat)
                .Include(t => t.ChongChiDinhs)
                .FirstOrDefaultAsync(t => t.MaThuoc == id);

            if (thuoc == null) return NotFound();
            ViewBag.HoatChats = await _context.HoatChats.OrderBy(h => h.TenHoatChat).ToListAsync();
            return View(thuoc);
        }

        [Authorize(Roles = "Admin,Dược sĩ")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Thuoc model,
            List<int>? maHoatChats, List<string>? hamLuongs,
            List<string>? tenBenhChongChiDinhs, List<string>? ghiChuChongChiDinhs)
        {
            if (id != model.MaThuoc) return BadRequest();

            ModelState.Remove("ThuocHoatChats");
            ModelState.Remove("ChongChiDinhs");
            ModelState.Remove("TuongTacA");
            ModelState.Remove("TuongTacB");
            ModelState.Remove("ChiTietDanhGias");

            if (!ModelState.IsValid)
            {
                ViewBag.HoatChats = await _context.HoatChats.OrderBy(h => h.TenHoatChat).ToListAsync();
                return View(model);
            }

            var existing = await _context.Thuocs.FindAsync(id);
            if (existing == null) return NotFound();

            existing.TenThuoc = model.TenThuoc;
            existing.TenBietDuoc = model.TenBietDuoc;
            existing.DangBaoChe = model.DangBaoChe;
            existing.LieuDungChuan = model.LieuDungChuan;
            existing.TinhTrangThuoc = model.TinhTrangThuoc;
            existing.MoTaChiTiet = model.MoTaChiTiet;

            var thcOld = await _context.ThuocHoatChats.Where(t => t.MaThuoc == id).ToListAsync();
            _context.ThuocHoatChats.RemoveRange(thcOld);

            if (maHoatChats != null)
            {
                for (int i = 0; i < maHoatChats.Count; i++)
                {
                    if (maHoatChats[i] > 0)
                    {
                        _context.ThuocHoatChats.Add(new ThuocHoatChat
                        {
                            MaThuoc = id,
                            MaHoatChat = maHoatChats[i],
                            HamLuong = hamLuongs != null && i < hamLuongs.Count ? hamLuongs[i] : null
                        });
                    }
                }
            }

            var ccdOld = await _context.ChongChiDinhs.Where(c => c.MaThuoc == id).ToListAsync();
            _context.ChongChiDinhs.RemoveRange(ccdOld);

            if (tenBenhChongChiDinhs != null)
            {
                for (int i = 0; i < tenBenhChongChiDinhs.Count; i++)
                {
                    if (!string.IsNullOrWhiteSpace(tenBenhChongChiDinhs[i]))
                    {
                        _context.ChongChiDinhs.Add(new ChongChiDinh
                        {
                            MaThuoc = id,
                            TenBenhChongChiDinh = tenBenhChongChiDinhs[i],
                            GhiChu = ghiChuChongChiDinhs != null && i < ghiChuChongChiDinhs.Count ? ghiChuChongChiDinhs[i] : null
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Cập nhật thuốc thành công!";
            return RedirectToAction(nameof(Details), new { id });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var thuoc = await _context.Thuocs.FindAsync(id);
            if (thuoc == null) return NotFound();
            _context.Thuocs.Remove(thuoc);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã xóa thuốc.";
            return RedirectToAction(nameof(Index));
        }
    }
}
