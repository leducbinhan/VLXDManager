using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VLXDManager.Data;
using VLXDManager.Models;

namespace VLXDManager.Controllers
{
    public class VatTuController : Controller
    {
        private readonly VLXDDbContext _context;

        public VatTuController(VLXDDbContext context) => _context = context;

        // GET: VatTu - Danh sách + tìm kiếm + phân trang
        public async Task<IActionResult> Index(VatTuSearchViewModel search)
        {
            ViewBag.DanhMucLoai = await _context.DanhMucLoai
                .Where(d => d.TrangThai)
                .Select(d => new SelectListItem
                {
                    Value = d.MaLoai.ToString(),
                    Text = d.TenLoai
                })
                .ToListAsync();
            search.DanhMucLoai = await _context.DanhMucLoai.Where(d => d.TrangThai).ToListAsync();

            var query = _context.VatTu.Include(v => v.DanhMucLoai).Where(v => v.TrangThai).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search.TuKhoa))
                query = query.Where(v => v.TenVatTu.Contains(search.TuKhoa) || v.MaHang.Contains(search.TuKhoa) || (v.NhaCungCap != null && v.NhaCungCap.Contains(search.TuKhoa)));

            if (search.MaLoai.HasValue && search.MaLoai > 0)
                query = query.Where(v => v.MaLoai == search.MaLoai);

            if (search.ChiSapHet == true)
                query = query.Where(v => v.SoLuongTon <= v.SoLuongMin);

            search.TotalCount = await query.CountAsync();
            search.VatTu = await query
                .OrderByDescending(v => v.NgayCapNhat)
                .Skip((search.Page - 1) * search.PageSize)
                .Take(search.PageSize)
                .ToListAsync();

            return View(search);
        }

        // GET: VatTu/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var vatTu = await _context.VatTu
                .Include(v => v.DanhMucLoai)
                .FirstOrDefaultAsync(v => v.MaVatTu == id);
            if (vatTu == null) return NotFound();
            return View(vatTu);
        }

        // GET: VatTu/Create
        public async Task<IActionResult> Create()
        {
            var danhMuc = await _context.DanhMucLoai
                .Where(d => d.TrangThai)
                .ToListAsync();

            ViewBag.DanhMucLoai = new SelectList(danhMuc, "MaLoai", "TenLoai");

            return View(new VatTu());
        }

        // POST: VatTu/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VatTu vatTu)
        {
            if (await _context.VatTu.AnyAsync(v => v.MaHang == vatTu.MaHang))
            {
                ModelState.AddModelError("MaHang", "Mã hàng đã tồn tại");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    vatTu.TrangThai = true;
                    vatTu.NgayTao = DateTime.Now;
                    vatTu.NgayCapNhat = DateTime.Now;

                    _context.Add(vatTu);

                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Thêm vật tư thành công";

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.InnerException?.Message ?? ex.Message);
                }
            }

            ViewBag.DanhMucLoai = new SelectList(
                await _context.DanhMucLoai
                    .Where(d => d.TrangThai)
                    .ToListAsync(),
                "MaLoai",
                "TenLoai",
                vatTu.MaLoai
            );

            return View(vatTu);
        }

        // GET: VatTu/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var vatTu = await _context.VatTu.FindAsync(id);
            if (vatTu == null) return NotFound();

            ViewBag.DanhMucLoai = new SelectList(
                await _context.DanhMucLoai.Where(d => d.TrangThai).ToListAsync(),
                "MaLoai", "TenLoai", vatTu.MaLoai);
            return View(vatTu);
        }

        // POST: VatTu/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VatTu vatTu)
        {
            if (id != vatTu.MaVatTu) return BadRequest();

            if (await _context.VatTu.AnyAsync(v => v.MaHang == vatTu.MaHang && v.MaVatTu != id))
                ModelState.AddModelError("MaHang", "Mã hàng đã tồn tại");

            if (ModelState.IsValid)
            {
                vatTu.NgayCapNhat = DateTime.Now;
                _context.Update(vatTu);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Đã cập nhật vật tư <b>{vatTu.TenVatTu}</b> thành công!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.DanhMucLoai = new SelectList(
                await _context.DanhMucLoai.Where(d => d.TrangThai).ToListAsync(),
                "MaLoai", "TenLoai", vatTu.MaLoai);
            return View(vatTu);
        }

        // POST: VatTu/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var vatTu = await _context.VatTu.FindAsync(id);
            if (vatTu == null) return NotFound();

            // Kiểm tra đã có trong đơn hàng chưa
            if (await _context.ChiTietDonHang.AnyAsync(c => c.MaVatTu == id))
            {
                TempData["Error"] = "Không thể xóa vật tư đã có trong đơn hàng!";
                return RedirectToAction(nameof(Index));
            }

            vatTu.TrangThai = false; // Soft delete
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Đã xóa vật tư <b>{vatTu.TenVatTu}</b>!";
            return RedirectToAction(nameof(Index));
        }

        // API: Lấy giá bán theo mã vật tư (dùng cho AJAX)
        [HttpGet]
        public async Task<IActionResult> GetGiaBan(int maVatTu)
        {
            var vt = await _context.VatTu.FindAsync(maVatTu);
            if (vt == null) return NotFound();
            return Json(new { giaBan = vt.GiaBan, donVi = vt.DonVi, ton = vt.SoLuongTon });
        }
    }
}