using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VLXDManager.Data;
using VLXDManager.Models;

namespace VLXDManager.Controllers
{
  
    public class ThanhVienController : Controller
    {
        private readonly VLXDDbContext _db;
        public ThanhVienController(VLXDDbContext db) => _db = db;

        // ── Danh sách + Tìm kiếm ──────────────────────────────
        public async Task<IActionResult> Index(string? q, string? chucVu, int p = 1)
        {
            int ps = 10;
            var query = _db.ThanhVien.Where(t => t.TrangThai).AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(t => t.HoTen.Contains(q) ||
                    t.MaTVCode.Contains(q) ||
                    (t.SoDienThoai != null && t.SoDienThoai.Contains(q)));

            if (!string.IsNullOrWhiteSpace(chucVu))
                query = query.Where(t => t.ChucVu == chucVu);

            ViewBag.Q = q;
            ViewBag.ChucVu = chucVu;
            ViewBag.Page = p;
            ViewBag.Total = await query.CountAsync();
            ViewBag.TotalPg = (int)Math.Ceiling((double)ViewBag.Total / ps);

            // Danh sách chức vụ để lọc
            ViewBag.DsChucVu = await _db.ThanhVien
                .Where(t => t.TrangThai && t.ChucVu != null)
                .Select(t => t.ChucVu!).Distinct().OrderBy(x => x).ToListAsync();

            var list = await query
                .OrderBy(t => t.ChucVu).ThenBy(t => t.HoTen)
                .Skip((p - 1) * ps).Take(ps).ToListAsync();

            return View(list);
        }

        // ── Chi tiết ──────────────────────────────────────────
        public async Task<IActionResult> Details(int id)
        {
            var tv = await _db.ThanhVien.FindAsync(id);
            return tv == null ? NotFound() : View(tv);
        }

        // ── Thêm mới ──────────────────────────────────────────
        public async Task<IActionResult> Create()
        {
            var cnt = await _db.ThanhVien.CountAsync() + 1;
            ViewBag.MaCode = $"TV{cnt:D3}";
            return View(new ThanhVien { NgayVaoLam = DateTime.Today });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ThanhVien m)
        {
            if (await _db.ThanhVien.AnyAsync(t => t.MaTVCode == m.MaTVCode))
                ModelState.AddModelError("MaTVCode", "Mã thành viên đã tồn tại");

            if (!ModelState.IsValid) return View(m);

            m.NgayTao = DateTime.Now;
            _db.ThanhVien.Add(m);
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Đã thêm thành viên <b>{m.HoTen}</b>!";
            return RedirectToAction(nameof(Index));
        }

        // ── Chỉnh sửa ─────────────────────────────────────────
        public async Task<IActionResult> Edit(int id)
        {
            var tv = await _db.ThanhVien.FindAsync(id);
            return tv == null ? NotFound() : View(tv);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ThanhVien m)
        {
            if (id != m.MaTV) return BadRequest();
            if (await _db.ThanhVien.AnyAsync(t => t.MaTVCode == m.MaTVCode && t.MaTV != id))
                ModelState.AddModelError("MaTVCode", "Mã thành viên đã tồn tại");
            if (!ModelState.IsValid) return View(m);

            _db.Update(m);
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Đã cập nhật <b>{m.HoTen}</b>!";
            return RedirectToAction(nameof(Index));
        }

        // ── Xoá (soft delete) ─────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var tv = await _db.ThanhVien.FindAsync(id);
            if (tv == null) return NotFound();
            tv.TrangThai = false;
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Đã xoá <b>{tv.HoTen}</b>!";
            return RedirectToAction(nameof(Index));
        }
    }
}