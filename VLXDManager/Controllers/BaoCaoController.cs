using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VLXDManager.Data;
using VLXDManager.Models;

namespace VLXDManager.Controllers
{
    public class BaoCaoController : Controller
    {
        private readonly VLXDDbContext _context;
        public BaoCaoController(VLXDDbContext context) => _context = context;

        // Báo cáo doanh thu
        public async Task<IActionResult> DoanhThu(int? nam, int? thangTu, int? thangDen)
        {
            nam ??= DateTime.Now.Year;
            thangTu ??= 1;
            thangDen ??= 12;

            ViewBag.Nam = nam;
            ViewBag.ThangTu = thangTu;
            ViewBag.ThangDen = thangDen;

            var data = await _context.DonHang
                .Where(d => d.NgayBan.Year == nam &&
                    d.NgayBan.Month >= thangTu &&
                    d.NgayBan.Month <= thangDen &&
                    d.TrangThai != "Đã hủy")
                .GroupBy(d => d.NgayBan.Month)
                .Select(g => new DoanhThuThang
                {
                    Thang = g.Key,
                    SoDonHang = g.Count(),
                    DoanhThu = g.Sum(d => d.ThanhTien),
                    DaThu = g.Sum(d => d.DaThanhToan),
                    ChuaThu = g.Sum(d => d.ConLai)
                })
                .OrderBy(x => x.Thang)
                .ToListAsync();

            // Fill các tháng thiếu
            var fullData = new List<DoanhThuThang>();
            for (int t = thangTu.Value; t <= thangDen.Value; t++)
            {
                var existing = data.FirstOrDefault(d => d.Thang == t);
                fullData.Add(existing ?? new DoanhThuThang { Thang = t });
            }

            return View(fullData);
        }

        // Báo cáo tồn kho
        public async Task<IActionResult> TonKho(int? maLoai)
        {
            var query = _context.VatTu.Include(v => v.DanhMucLoai).Where(v => v.TrangThai).AsQueryable();
            if (maLoai.HasValue && maLoai > 0)
                query = query.Where(v => v.MaLoai == maLoai);

            ViewBag.DanhMucLoais = await _context.DanhMucLoai.Where(d => d.TrangThai).ToListAsync();
            ViewBag.MaLoai = maLoai;

            var list = await query.OrderBy(v => v.MaLoai).ThenBy(v => v.TenVatTu).ToListAsync();

            ViewBag.TongGiaTri = list.Sum(v => v.SoLuongTon * v.GiaNhap);
            ViewBag.SapHet = list.Count(v => v.SoLuongTon <= v.SoLuongMin);

            return View(list);
        }

        // Top vật tư bán chạy
        public async Task<IActionResult> BanChay(int? nam)
        {
            nam ??= DateTime.Now.Year;
            ViewBag.Nam = nam;

            var data = await _context.ChiTietDonHang
                .Include(ct => ct.VatTu!).ThenInclude(v => v!.DanhMucLoai)
                .Include(ct => ct.DonHang)
                .Where(ct => ct.DonHang!.NgayBan.Year == nam && ct.DonHang.TrangThai != "Đã hủy")
                .GroupBy(ct => new { ct.MaVatTu, ct.VatTu!.TenVatTu, ct.VatTu.DonVi, LoaiName = ct.VatTu.DanhMucLoai!.TenLoai })
                .Select(g => new SanPhamBanChayViewModel
                {
                    TenVatTu = g.Key.TenVatTu,
                    DonVi = g.Key.DonVi,
                    LoaiName = g.Key.LoaiName,
                    TongSoLuong = g.Sum(ct => ct.SoLuong),
                    TongDoanhThu = g.Sum(ct => ct.ThanhTien)
                })
                .OrderByDescending(x => x.TongDoanhThu)
                .Take(20)
                .ToListAsync();

            return View(data);
        }

        // Báo cáo công nợ
        public async Task<IActionResult> CongNo()
        {
            var data = await _context.DonHang
                .Include(d => d.KhachHang)
                .Where(d => d.ConLai > 0 && d.TrangThai != "Đã hủy")
                .OrderByDescending(d => d.ConLai)
                .ToListAsync();

            ViewBag.TongCongNo = data.Sum(d => d.ConLai);
            return View(data);
        }
    }
}