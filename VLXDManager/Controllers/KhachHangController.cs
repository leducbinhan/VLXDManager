using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VLXDManager.Data;
using VLXDManager.Models;

namespace VLXDManager.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly VLXDDbContext _context;
        public KhachHangController(VLXDDbContext context) => _context = context;

        public async Task<IActionResult> Index(string? tuKhoa, int page = 1)
        {
            int pageSize = 10;
            var query = _context.KhachHang.Where(k => k.TrangThai).AsQueryable();

            if (!string.IsNullOrWhiteSpace(tuKhoa))
                query = query.Where(k => k.TenKH.Contains(tuKhoa) ||
                    k.MaKHCode.Contains(tuKhoa) ||
                    (k.SoDienThoai != null && k.SoDienThoai.Contains(tuKhoa)));

            ViewBag.TuKhoa = tuKhoa;
            ViewBag.TotalCount = await query.CountAsync();
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)ViewBag.TotalCount / pageSize);

            var list = await query.OrderByDescending(k => k.NgayTao)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return View(list);
        }

        public async Task<IActionResult> Details(int id)
        {
            var kh = await _context.KhachHang.FirstOrDefaultAsync(k => k.MaKH == id);
            if (kh == null) return NotFound();

            ViewBag.DonHang = await _context.DonHang
                .Where(d => d.MaKH == id)
                .OrderByDescending(d => d.NgayBan)
                .Take(10)
                .ToListAsync();
            ViewBag.TongDoanhThu = await _context.DonHang
                .Where(d => d.MaKH == id && d.TrangThai != "Đã hủy")
                .SumAsync(d => (decimal?)d.ThanhTien) ?? 0;

            return View(kh);
        }

        public async Task<IActionResult> Create()
        {
            // Tự sinh mã KH
            var count = await _context.KhachHang.CountAsync() + 1;
            ViewBag.MaKHCode = $"KH{count:D3}";
            return View(new KhachHang());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KhachHang kh)
        {
            if (await _context.KhachHang.AnyAsync(k => k.MaKHCode == kh.MaKHCode))
                ModelState.AddModelError("MaKHCode", "Mã khách hàng đã tồn tại");

            if (ModelState.IsValid)
            {
                kh.NgayTao = DateTime.Now;
                _context.Add(kh);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Đã thêm khách hàng <b>{kh.TenKH}</b>!";
                return RedirectToAction(nameof(Index));
            }
            return View(kh);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var kh = await _context.KhachHang.FindAsync(id);
            if (kh == null) return NotFound();
            return View(kh);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, KhachHang kh)
        {
            if (id != kh.MaKH) return BadRequest();

            if (ModelState.IsValid)
            {
                _context.Update(kh);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Đã cập nhật khách hàng <b>{kh.TenKH}</b>!";
                return RedirectToAction(nameof(Index));
            }
            return View(kh);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var kh = await _context.KhachHang.FindAsync(id);
            if (kh == null) return NotFound();

            if (await _context.DonHang.AnyAsync(d => d.MaKH == id))
            {
                TempData["Error"] = "Không thể xóa khách hàng đã có đơn hàng!";
                return RedirectToAction(nameof(Index));
            }

            kh.TrangThai = false;
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Đã xóa khách hàng <b>{kh.TenKH}</b>!";
            return RedirectToAction(nameof(Index));
        }

        // AJAX search gợi ý khách hàng
        [HttpGet]
        public async Task<IActionResult> Search(string q)
        {
            var list = await _context.KhachHang
                .Where(k => k.TrangThai && (k.TenKH.Contains(q) || k.SoDienThoai!.Contains(q)))
                .Select(k => new { k.MaKH, k.TenKH, k.SoDienThoai, k.DiaChi })
                .Take(10)
                .ToListAsync();
            return Json(list);
        }
    }
}