using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VLXDManager.Data;
using VLXDManager.Models;

namespace VLXDManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly VLXDDbContext _context;

        public HomeController(VLXDDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var startOfYear = new DateTime(today.Year, 1, 1);

            var vm = new DashboardViewModel
            {
                TongVatTu = await _context.VatTu.CountAsync(v => v.TrangThai),
                VatTuSapHet = await _context.VatTu.CountAsync(v => v.TrangThai && v.SoLuongTon <= v.SoLuongMin),
                TongKhachHang = await _context.KhachHang.CountAsync(k => k.TrangThai),
                DonHangHomNay = await _context.DonHang.CountAsync(d => d.NgayBan.Date == today),
                DoanhThuHomNay = await _context.DonHang
                    .Where(d => d.NgayBan.Date == today && d.TrangThai != "Đã hủy")
                    .SumAsync(d => (decimal?)d.ThanhTien) ?? 0,
                DoanhThuThangNay = await _context.DonHang
                    .Where(d => d.NgayBan >= startOfMonth && d.TrangThai != "Đã hủy")
                    .SumAsync(d => (decimal?)d.ThanhTien) ?? 0,
                DoanhThuNamNay = await _context.DonHang
                    .Where(d => d.NgayBan >= startOfYear && d.TrangThai != "Đã hủy")
                    .SumAsync(d => (decimal?)d.ThanhTien) ?? 0,
                DonHangGanDay = await _context.DonHang
                    .Include(d => d.KhachHang)
                    .OrderByDescending(d => d.NgayTao)
                    .Take(8)
                    .ToListAsync(),
                VatTuSapHetList = await _context.VatTu
                    .Include(v => v.DanhMucLoai)
                    .Where(v => v.TrangThai && v.SoLuongTon <= v.SoLuongMin)
                    .OrderBy(v => v.SoLuongTon)
                    .Take(6)
                    .ToListAsync()
            };

            // Doanh thu 12 tháng gần nhất
            vm.DoanhThuTheoThang = await _context.DonHang
                    .Where(d => d.NgayBan.Year == today.Year && d.TrangThai != "Đã hủy")
                    .GroupBy(d => new { d.NgayBan.Year, d.NgayBan.Month })
                    .Select(g => new DoanhThuThang
                    {
                        Thang = g.Key.Month,
                        SoDonHang = g.Count(),
                        DoanhThu = g.Sum(d => d.ThanhTien),
                        DaThu = g.Sum(d => d.DaThanhToan),
                        ChuaThu = g.Sum(d => d.ConLai)
                    })
                    .OrderBy(x => x.Thang)
                    .ToListAsync();

            return View(vm);
        }
    }
}