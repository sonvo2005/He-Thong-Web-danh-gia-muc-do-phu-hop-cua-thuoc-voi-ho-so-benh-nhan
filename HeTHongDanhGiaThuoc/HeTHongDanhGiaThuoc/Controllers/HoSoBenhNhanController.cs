using HeTHongDanhGiaThuoc.Data;
using HeTHongDanhGiaThuoc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HeTHongDanhGiaThuoc.Controllers
{
    [Authorize]
    public class HoSoBenhNhanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HoSoBenhNhanController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ================= LIST =================
        public async Task<IActionResult> Index(string? search, int page = 1)
        {
            int pageSize = 10;

            var query = _context.HoSoBenhNhans
                .Include(x => x.NguoiTao)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    x.HoTen.Contains(search) ||
                    x.MaDinhDanhBenhNhan.Contains(search));

                ViewBag.Search = search;
            }

            int total = await query.CountAsync();

            var data = await query
                .OrderByDescending(x => x.NgayTiepNhan)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.Total = total;

            return View(data);
        }

        // ================= DETAILS =================
        public async Task<IActionResult> Details(int id)
        {
            var data = await _context.HoSoBenhNhans
                .Include(x => x.NguoiTao)
                .Include(x => x.BenhNens)
                .Include(x => x.DiUngThuocs).ThenInclude(x => x.HoatChat)
                .Include(x => x.DanhGias)
                    .ThenInclude(x => x.NguoiDanhGia)
                .Include(x => x.DanhGias)
                    .ThenInclude(x => x.ChiTietDanhGias)
                        .ThenInclude(x => x.Thuoc)
                .FirstOrDefaultAsync(x => x.MaBenhNhan == id);

            if (data == null) return NotFound();

            return View(data);
        }

        // ================= CREATE =================
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            HoSoBenhNhan model,
            List<string>? tenBenhNens,
            List<DateTime>? ngayChanDoans,
            List<int>? maHoatChats,
            List<string>? mucDoDiUngs)
        {
            ModelState.Remove(nameof(model.NguoiTao));
            ModelState.Remove(nameof(model.BenhNens));
            ModelState.Remove(nameof(model.DiUngThuocs));
            ModelState.Remove(nameof(model.DanhGias));

            if (!ModelState.IsValid)
                return View(model);

            bool exist = await _context.HoSoBenhNhans
                .AnyAsync(x => x.MaDinhDanhBenhNhan == model.MaDinhDanhBenhNhan);

            if (exist)
            {
                ModelState.AddModelError("", "Mã định danh đã tồn tại");
                return View(model);
            }

            model.MaNguoiTao = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            model.NgayTiepNhan = DateTime.Now;

            _context.HoSoBenhNhans.Add(model);
            await _context.SaveChangesAsync();

            // ===== BỆNH NỀN =====
            if (tenBenhNens != null)
            {
                for (int i = 0; i < tenBenhNens.Count; i++)
                {
                    if (!string.IsNullOrWhiteSpace(tenBenhNens[i]))
                    {
                        _context.BenhNens.Add(new BenhNen
                        {
                            MaBenhNhan = model.MaBenhNhan,
                            TenBenhNen = tenBenhNens[i],
                            NgayChanDoan = (ngayChanDoans != null && i < ngayChanDoans.Count)
                                ? ngayChanDoans[i]
                                : DateTime.Now
                        });
                    }
                }
            }

            // ===== DỊ ỨNG =====
            if (maHoatChats != null)
            {
                for (int i = 0; i < maHoatChats.Count; i++)
                {
                    if (maHoatChats[i] > 0)
                    {
                        _context.DiUngThuocs.Add(new DiUngThuoc
                        {
                            MaBenhNhan = model.MaBenhNhan,
                            MaHoatChat = maHoatChats[i],
                            MucDoDiUng = (mucDoDiUngs != null && i < mucDoDiUngs.Count)
                                ? mucDoDiUngs[i]
                                : ""
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Tạo hồ sơ thành công";
            return RedirectToAction(nameof(Details), new { id = model.MaBenhNhan });
        }

        // ================= EDIT =================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var data = await _context.HoSoBenhNhans
                .Include(x => x.BenhNens)
                .Include(x => x.DiUngThuocs)
                    .ThenInclude(x => x.HoatChat)
                .FirstOrDefaultAsync(x => x.MaBenhNhan == id);

            if (data == null) return NotFound();

            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            HoSoBenhNhan model,
            List<string>? tenBenhNens,
            List<DateTime>? ngayChanDoans,
            List<int>? maHoatChats,
            List<string>? mucDoDiUngs)
        {
            if (id != model.MaBenhNhan)
                return BadRequest();

            ModelState.Remove(nameof(model.NguoiTao));
            ModelState.Remove(nameof(model.BenhNens));
            ModelState.Remove(nameof(model.DiUngThuocs));
            ModelState.Remove(nameof(model.DanhGias));

            if (!ModelState.IsValid)
                return View(model);

            var entity = await _context.HoSoBenhNhans
                .FirstOrDefaultAsync(x => x.MaBenhNhan == id);

            if (entity == null) return NotFound();

            // ===== UPDATE BASIC =====
            entity.HoTen = model.HoTen;
            entity.MaDinhDanhBenhNhan = model.MaDinhDanhBenhNhan;
            entity.GioiTinh = model.GioiTinh;
            entity.NgaySinh = model.NgaySinh;
            entity.DiaChi = model.DiaChi;

            // ===== CLEAR OLD DATA =====
            _context.BenhNens.RemoveRange(
                _context.BenhNens.Where(x => x.MaBenhNhan == id));

            _context.DiUngThuocs.RemoveRange(
                _context.DiUngThuocs.Where(x => x.MaBenhNhan == id));

            // ===== ADD NEW BỆNH NỀN =====
            if (tenBenhNens != null)
            {
                for (int i = 0; i < tenBenhNens.Count; i++)
                {
                    if (!string.IsNullOrWhiteSpace(tenBenhNens[i]))
                    {
                        _context.BenhNens.Add(new BenhNen
                        {
                            MaBenhNhan = id,
                            TenBenhNen = tenBenhNens[i],
                            NgayChanDoan = (ngayChanDoans != null && i < ngayChanDoans.Count)
                                ? ngayChanDoans[i]
                                : DateTime.Now
                        });
                    }
                }
            }

            // ===== ADD NEW DỊ ỨNG =====
            if (maHoatChats != null)
            {
                for (int i = 0; i < maHoatChats.Count; i++)
                {
                    if (maHoatChats[i] > 0)
                    {
                        _context.DiUngThuocs.Add(new DiUngThuoc
                        {
                            MaBenhNhan = id,
                            MaHoatChat = maHoatChats[i],
                            MucDoDiUng = (mucDoDiUngs != null && i < mucDoDiUngs.Count)
                                ? mucDoDiUngs[i]
                                : ""
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Cập nhật thành công";
            return RedirectToAction(nameof(Details), new { id });
        }

        // ================= DELETE =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,BacSi")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.HoSoBenhNhans
                .FirstOrDefaultAsync(x => x.MaBenhNhan == id);

            if (entity == null) return NotFound();

            _context.HoSoBenhNhans.Remove(entity);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã xóa hồ sơ";
            return RedirectToAction(nameof(Index));
        }
    }
}