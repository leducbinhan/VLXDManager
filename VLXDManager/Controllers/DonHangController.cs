using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VLXDManager.Data;
using VLXDManager.Models;

namespace VLXDManager.Controllers
{
    public class DonHangController : Controller
    {
        private readonly VLXDDbContext _context;

        public DonHangController(VLXDDbContext context) => _context = context;

        // GET: DonHang
        public async Task<IActionResult> Index(DonHangSearchViewModel search)
        {
            var query = _context.DonHang.Include(d => d.KhachHang).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search.TuKhoa))
                query = query.Where(d => d.SoDH.Contains(search.TuKhoa) ||
                    (d.KhachHang != null && d.KhachHang.TenKH.Contains(search.TuKhoa)));

            if (!string.IsNullOrWhiteSpace(search.TrangThai))
                query = query.Where(d => d.TrangThai == search.TrangThai);

            if (search.TuNgay.HasValue)
                query = query.Where(d => d.NgayBan >= search.TuNgay.Value);

            if (search.DenNgay.HasValue)
                query = query.Where(d => d.NgayBan <= search.DenNgay.Value.AddDays(1));

            search.TotalCount = await query.CountAsync();
            search.DonHang = await query
                .OrderByDescending(d => d.NgayTao)
                .Skip((search.Page - 1) * search.PageSize)
                .Take(search.PageSize)
                .ToListAsync();

            return View(search);
        }

        // GET: DonHang/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var donHang = await _context.DonHang
                .Include(d => d.KhachHang)
                .Include(d => d.ChiTietDonHang)
                    .ThenInclude(ct => ct.VatTu)
                .FirstOrDefaultAsync(d => d.MaDH == id);

            if (donHang == null) return NotFound();
            return View(donHang);
        }

        // GET: DonHang/Create
        public async Task<IActionResult> Create()
        {
            var vm = new CreateDonHangViewModel
            {
                KhachHang = await _context.KhachHang.Where(k => k.TrangThai).ToListAsync(),
                VatTu = await _context.VatTu.Include(v => v.DanhMucLoai).Where(v => v.TrangThai).ToListAsync()
            };

            // Tạo số đơn hàng tự động
            var year = DateTime.Now.Year.ToString();
            var month = DateTime.Now.Month.ToString("D2");
            var count = await _context.DonHang.CountAsync(d =>
                d.NgayTao.Year == DateTime.Now.Year && d.NgayTao.Month == DateTime.Now.Month) + 1;
            vm.DonHang.SoDH = $"DH{year}{month}{count:D3}";
            vm.DonHang.NgayBan = DateTime.Now;

            return View(vm);
        }

        // POST: DonHang/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [FromForm] DonHang donHang,
            [FromForm] List<int> maVatTuList,
            [FromForm] List<int> soLuongList,
            [FromForm] List<decimal> donGiaList)
        {
            if (maVatTuList == null || !maVatTuList.Any() ||
                soLuongList == null || donGiaList == null ||
                maVatTuList.Count != soLuongList.Count || maVatTuList.Count != donGiaList.Count)
            {
                TempData["Error"] = "Vui lòng thêm ít nhất một vật tư hợp lệ vào đơn hàng!";
                return await ReloadCreateView(donHang);
            }

            // Tạo số đơn hàng nếu trùng
            if (await _context.DonHang.AnyAsync(d => d.SoDH == donHang.SoDH))
            {
                var year = DateTime.Now.Year.ToString();
                var month = DateTime.Now.Month.ToString("D2");
                var count = await _context.DonHang.CountAsync(d =>
                    d.NgayTao.Year == DateTime.Now.Year && d.NgayTao.Month == DateTime.Now.Month) + 1;
                donHang.SoDH = $"DH{year}{month}{count:D3}";
            }

            donHang.NgayTao = DateTime.Now;
            donHang.NgayCapNhat = DateTime.Now;

            // Tính tổng tiền
            decimal tongTien = 0;
            var chiTiets = new List<ChiTietDonHang>();

            for (int i = 0; i < maVatTuList.Count; i++)
            {
                if (maVatTuList[i] <= 0 || soLuongList[i] <= 0 || donGiaList[i] < 0)
                {
                    TempData["Error"] = "Thông tin vật tư trong đơn hàng không hợp lệ!";
                    return await ReloadCreateView(donHang);
                }

                var vatTu = await _context.VatTu.FindAsync(maVatTuList[i]);
                if (vatTu == null || !vatTu.TrangThai)
                {
                    TempData["Error"] = "Vật tư không tồn tại hoặc đã ngừng sử dụng!";
                    return await ReloadCreateView(donHang);
                }

                if (vatTu.SoLuongTon < soLuongList[i])
                {
                    TempData["Error"] = $"Vật tư {vatTu.TenVatTu} không đủ tồn kho. Tồn hiện tại: {vatTu.SoLuongTon} {vatTu.DonVi}.";
                    return await ReloadCreateView(donHang);
                }

                var tt = soLuongList[i] * donGiaList[i];
                tongTien += tt;
                chiTiets.Add(new ChiTietDonHang
                {
                    MaVatTu = maVatTuList[i],
                    SoLuong = soLuongList[i],
                    DonGia = donGiaList[i],
                    ThanhTien = tt
                });
            }

            donHang.TongTien = tongTien;
            donHang.ThanhTien = tongTien - donHang.ChietKhau;
            donHang.ConLai = donHang.ThanhTien - donHang.DaThanhToan;

            _context.DonHang.Add(donHang);
            await _context.SaveChangesAsync();

            // Lưu chi tiết
            foreach (var ct in chiTiets)
            {
                ct.MaDH = donHang.MaDH;
                _context.ChiTietDonHang.Add(ct);

                // Trừ tồn kho
                var vt = await _context.VatTu.FindAsync(ct.MaVatTu);
                if (vt != null)
                {
                    vt.SoLuongTon -= ct.SoLuong;
                    vt.NgayCapNhat = DateTime.Now;
                }
            }
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Đã tạo đơn hàng <b>{donHang.SoDH}</b> thành công!";
            return RedirectToAction(nameof(Details), new { id = donHang.MaDH });
        }

        // POST: DonHang/CapNhatTrangThai
        [HttpPost]
        public async Task<IActionResult> CapNhatTrangThai(int id, string trangThai)
        {
            var dh = await _context.DonHang.FindAsync(id);
            if (dh == null) return NotFound();

            // Nếu hủy đơn → hoàn lại tồn kho
            if (trangThai == "Đã hủy" && dh.TrangThai != "Đã hủy")
            {
                var chiTiets = await _context.ChiTietDonHang.Where(c => c.MaDH == id).ToListAsync();
                foreach (var ct in chiTiets)
                {
                    var vt = await _context.VatTu.FindAsync(ct.MaVatTu);
                    if (vt != null) vt.SoLuongTon += ct.SoLuong;
                }
            }

            dh.TrangThai = trangThai;
            dh.NgayCapNhat = DateTime.Now;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Đã cập nhật trạng thái đơn hàng <b>{dh.SoDH}</b>!";
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: DonHang/ThanhToan
        [HttpPost]
        public async Task<IActionResult> ThanhToan(int id, decimal soTienThem)
        {
            var dh = await _context.DonHang.FindAsync(id);
            if (dh == null) return NotFound();

            dh.DaThanhToan += soTienThem;
            dh.ConLai = dh.ThanhTien - dh.DaThanhToan;
            if (dh.ConLai <= 0) { dh.ConLai = 0; }
            dh.NgayCapNhat = DateTime.Now;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Đã ghi nhận thanh toán {soTienThem:N0}đ!";
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: In đơn hàng (preview PDF)
        public async Task<IActionResult> In(int id)
        {
            var donHang = await _context.DonHang
                .Include(d => d.KhachHang)
                .Include(d => d.ChiTietDonHang!)
                    .ThenInclude(ct => ct.VatTu)
                .FirstOrDefaultAsync(d => d.MaDH == id);

            if (donHang == null) return NotFound();
            return View(donHang);
        }

        private async Task<IActionResult> ReloadCreateView(DonHang donHang)
        {
            var vm = new CreateDonHangViewModel
            {
                DonHang = donHang,
                KhachHang = await _context.KhachHang.Where(k => k.TrangThai).ToListAsync(),
                VatTu = await _context.VatTu.Include(v => v.DanhMucLoai).Where(v => v.TrangThai).ToListAsync()
            };
            return View("Create", vm);
        }
    }
}