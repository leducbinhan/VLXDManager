using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VLXDManager.Models
{
    // =============================================
    // DANH MỤC LOẠI VẬT TƯ
    // =============================================
    public class DanhMucLoai
    {
        [Key]
        public int MaLoai { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên loại")]
        [StringLength(100)]
        [Display(Name = "Tên loại")]
        public string TenLoai { get; set; } = "";

        [StringLength(255)]
        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        [Display(Name = "Trạng thái")]
        public bool TrangThai { get; set; } = true;

        public DateTime NgayTao { get; set; } = DateTime.Now;

        public ICollection<VatTu>? VatTu { get; set; }
    }

    // =============================================
    // VẬT TƯ
    // =============================================
    public class VatTu
    {
        [Key]
        public int MaVatTu { get; set; }

        [Required]
        [Display(Name = "Loại vật tư")]
        public int MaLoai { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mã hàng")]
        [StringLength(50)]
        [Display(Name = "Mã hàng")]
        public string MaHang { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng nhập tên vật tư")]
        [StringLength(200)]
        [Display(Name = "Tên vật tư")]
        public string TenVatTu { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng nhập đơn vị")]
        [StringLength(50)]
        [Display(Name = "Đơn vị")]
        public string DonVi { get; set; } = "";

        [Required]
        [Range(0, double.MaxValue)]
        [Display(Name = "Giá nhập")]
        [DataType(DataType.Currency)]
        public decimal GiaNhap { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        [Display(Name = "Giá bán")]
        [DataType(DataType.Currency)]
        public decimal GiaBan { get; set; }

        [Display(Name = "Tồn kho")]
        public int SoLuongTon { get; set; }

        [Display(Name = "Tồn tối thiểu")]
        public int SoLuongMin { get; set; } = 10;

        [StringLength(200)]
        [Display(Name = "Nhà cung cấp")]
        public string? NhaCungCap { get; set; }

        [StringLength(500)]
        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        [StringLength(255)]
        public string? HinhAnh { get; set; }

        [Display(Name = "Trạng thái")]
        public bool TrangThai { get; set; } = true;

        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime NgayCapNhat { get; set; } = DateTime.Now;

        [ForeignKey("MaLoai")]
        public DanhMucLoai? DanhMucLoai { get; set; }
        public ICollection<ChiTietDonHang>? ChiTietDonHang { get; set; }
    }

    // =============================================
    // KHÁCH HÀNG
    // =============================================
    public class KhachHang
    {
        [Key]
        public int MaKH { get; set; }

        [Required(ErrorMessage = "Mã khách hàng là bắt buộc")]
        [StringLength(20)]
        [Display(Name = "Mã KH")]
        public string MaKHCode { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng nhập tên khách hàng")]
        [StringLength(200)]
        [Display(Name = "Tên khách hàng")]
        public string TenKH { get; set; } = "";

        [StringLength(20)]
        [Display(Name = "Số điện thoại")]
        public string? SoDienThoai { get; set; }

        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [StringLength(500)]
        [Display(Name = "Địa chỉ")]
        public string? DiaChi { get; set; }

        [StringLength(500)]
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        [Display(Name = "Trạng thái")]
        public bool TrangThai { get; set; } = true;

        public DateTime NgayTao { get; set; } = DateTime.Now;

        public ICollection<DonHang>? DonHang { get; set; }
    }

    // =============================================
    // ĐƠN HÀNG
    // =============================================
    public class DonHang
    {
        [Key]
        public int MaDH { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Số ĐH")]
        public string SoDH { get; set; } = "";

        [Display(Name = "Khách hàng")]
        public int? MaKH { get; set; }

        [Display(Name = "Ngày bán")]
        [DataType(DataType.DateTime)]
        public DateTime NgayBan { get; set; } = DateTime.Now;

        [Display(Name = "Tổng tiền")]
        [DataType(DataType.Currency)]
        public decimal TongTien { get; set; }

        [Display(Name = "Chiết khấu")]
        [DataType(DataType.Currency)]
        public decimal ChietKhau { get; set; }

        [Display(Name = "Thành tiền")]
        [DataType(DataType.Currency)]
        public decimal ThanhTien { get; set; }

        [Display(Name = "Đã thanh toán")]
        [DataType(DataType.Currency)]
        public decimal DaThanhToan { get; set; }

        [Display(Name = "Còn lại")]
        [DataType(DataType.Currency)]
        public decimal ConLai { get; set; }

        [StringLength(50)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = "Mới tạo";

        [StringLength(500)]
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        [StringLength(100)]
        [Display(Name = "Người bán")]
        public string? NguoiBan { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime NgayCapNhat { get; set; } = DateTime.Now;

        [ForeignKey("MaKH")]
        public KhachHang? KhachHang { get; set; }
        public ICollection<ChiTietDonHang>? ChiTietDonHang { get; set; }
    }

    // =============================================
    // CHI TIẾT ĐƠN HÀNG
    // =============================================
    public class ChiTietDonHang
    {
        [Key]
        public int MaCTDH { get; set; }

        public int MaDH { get; set; }
        public int MaVatTu { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        [Display(Name = "Số lượng")]
        public int SoLuong { get; set; }

        [Required]
        [Display(Name = "Đơn giá")]
        [DataType(DataType.Currency)]
        public decimal DonGia { get; set; }

        [Display(Name = "Thành tiền")]
        [DataType(DataType.Currency)]
        public decimal ThanhTien { get; set; }

        [StringLength(255)]
        public string? GhiChu { get; set; }

        [ForeignKey("MaDH")]
        public DonHang? DonHang { get; set; }

        [ForeignKey("MaVatTu")]
        public VatTu? VatTu { get; set; }
    }

    // =============================================
    // NHẬP KHO
    // =============================================
    public class NhapKho
    {
        [Key]
        public int MaNK { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Số phiếu")]
        public string SoPhieu { get; set; } = "";

        [StringLength(200)]
        [Display(Name = "Nhà cung cấp")]
        public string? NhaCungCap { get; set; }

        [Display(Name = "Ngày nhập")]
        [DataType(DataType.DateTime)]
        public DateTime NgayNhap { get; set; } = DateTime.Now;

        [Display(Name = "Tổng tiền")]
        [DataType(DataType.Currency)]
        public decimal TongTien { get; set; }

        [StringLength(500)]
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        [StringLength(100)]
        public string? NguoiNhap { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        public ICollection<ChiTietNhapKho>? ChiTietNhapKho { get; set; }
    }

    public class ChiTietNhapKho
    {
        [Key]
        public int MaCTNK { get; set; }
        public int MaNK { get; set; }
        public int MaVatTu { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }

        [ForeignKey("MaNK")]
        public NhapKho? NhapKho { get; set; }
        [ForeignKey("MaVatTu")]
        public VatTu? VatTu { get; set; }
    }

    // =============================================
    // VIEW MODELS (dùng cho giao diện)
    // =============================================
    public class DashboardViewModel
    {
        public int TongVatTu { get; set; }
        public int VatTuSapHet { get; set; }
        public int TongKhachHang { get; set; }
        public int DonHangHomNay { get; set; }
        public decimal DoanhThuHomNay { get; set; }
        public decimal DoanhThuThangNay { get; set; }
        public decimal DoanhThuNamNay { get; set; }
        public List<DonHang> DonHangGanDay { get; set; } = new();
        public List<VatTu> VatTuSapHetList { get; set; } = new();
        public List<DoanhThuThang> DoanhThuTheoThang { get; set; } = new();
    }

    public class DoanhThuThang
    {
        public int Thang { get; set; }
        public string TenThang => $"T{Thang}";
        public int SoDonHang { get; set; }
        public decimal DoanhThu { get; set; }
        public decimal DaThu { get; set; }
        public decimal ChuaThu { get; set; }
    }

    public class VatTuSearchViewModel
    {
        public string? TuKhoa { get; set; }
        public int? MaLoai { get; set; }
        public bool? ChiSapHet { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }
        public List<VatTu> VatTu { get; set; } = new();
        public List<DanhMucLoai> DanhMucLoai { get; set; } = new();
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }

    public class DonHangSearchViewModel
    {
        public string? TuKhoa { get; set; }
        public string? TrangThai { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }
        public List<DonHang> DonHang { get; set; } = new();
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }

    public class CreateDonHangViewModel
    {
        public DonHang DonHang { get; set; } = new();
        public List<ChiTietDonHang> ChiTiets { get; set; } = new();
        public List<KhachHang> KhachHang { get; set; } = new();
        public List<VatTu> VatTu { get; set; } = new();
    }

    public class SanPhamBanChayViewModel
    {
        public string TenVatTu { get; set; } = "";
        public string DonVi { get; set; } = "";
        public string LoaiName { get; set; } = "";
        public int TongSoLuong { get; set; }
        public decimal TongDoanhThu { get; set; }
    }
    public class ThanhVien
    {
        [Key]
        public int MaTV { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mã thành viên")]
        [StringLength(20)]
        [Display(Name = "Mã TV")]
        public string MaTVCode { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [StringLength(150)]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = "";

        [StringLength(100)]
        [Display(Name = "Chức vụ")]
        public string? ChucVu { get; set; }

        [StringLength(20)]
        [Display(Name = "Số điện thoại")]
        public string? SoDienThoai { get; set; }

        [StringLength(100)]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [StringLength(500)]
        [Display(Name = "Địa chỉ")]
        public string? DiaChi { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateTime? NgaySinh { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ngày vào làm")]
        public DateTime? NgayVaoLam { get; set; }

        [Display(Name = "Lương cơ bản")]
        [DataType(DataType.Currency)]
        public decimal LuongCoBan { get; set; }

        [StringLength(500)]
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        [StringLength(255)]
        public string? HinhAnh { get; set; }

        [Display(Name = "Trạng thái")]
        public bool TrangThai { get; set; } = true;

        public DateTime NgayTao { get; set; } = DateTime.Now;
    }
}