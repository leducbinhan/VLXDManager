using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VLXDManager.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DanhMucLoai",
                columns: table => new
                {
                    MaLoai = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenLoai = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucLoai", x => x.MaLoai);
                });

            migrationBuilder.CreateTable(
                name: "KhachHang",
                columns: table => new
                {
                    MaKH = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKHCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TenKH = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhachHang", x => x.MaKH);
                });

            migrationBuilder.CreateTable(
                name: "NhapKho",
                columns: table => new
                {
                    MaNK = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoPhieu = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NhaCungCap = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NgayNhap = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TongTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NguoiNhap = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhapKho", x => x.MaNK);
                });

            migrationBuilder.CreateTable(
                name: "ThanhVien",
                columns: table => new
                {
                    MaTV = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaTVCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ChucVu = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SoDienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayVaoLam = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LuongCoBan = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    HinhAnh = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThanhVien", x => x.MaTV);
                });

            migrationBuilder.CreateTable(
                name: "VatTu",
                columns: table => new
                {
                    MaVatTu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaLoai = table.Column<int>(type: "int", nullable: false),
                    MaHang = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TenVatTu = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DonVi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GiaNhap = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    GiaBan = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    SoLuongTon = table.Column<int>(type: "int", nullable: false),
                    SoLuongMin = table.Column<int>(type: "int", nullable: false),
                    NhaCungCap = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    HinhAnh = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VatTu", x => x.MaVatTu);
                    table.ForeignKey(
                        name: "FK_VatTu_DanhMucLoai_MaLoai",
                        column: x => x.MaLoai,
                        principalTable: "DanhMucLoai",
                        principalColumn: "MaLoai",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DonHang",
                columns: table => new
                {
                    MaDH = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoDH = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaKH = table.Column<int>(type: "int", nullable: true),
                    NgayBan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TongTien = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    ChietKhau = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    ThanhTien = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    DaThanhToan = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    ConLai = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NguoiBan = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonHang", x => x.MaDH);
                    table.ForeignKey(
                        name: "FK_DonHang_KhachHang_MaKH",
                        column: x => x.MaKH,
                        principalTable: "KhachHang",
                        principalColumn: "MaKH",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietNhapKho",
                columns: table => new
                {
                    MaCTNK = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNK = table.Column<int>(type: "int", nullable: false),
                    MaVatTu = table.Column<int>(type: "int", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    ThanhTien = table.Column<decimal>(type: "decimal(12,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietNhapKho", x => x.MaCTNK);
                    table.ForeignKey(
                        name: "FK_ChiTietNhapKho_NhapKho_MaNK",
                        column: x => x.MaNK,
                        principalTable: "NhapKho",
                        principalColumn: "MaNK",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietNhapKho_VatTu_MaVatTu",
                        column: x => x.MaVatTu,
                        principalTable: "VatTu",
                        principalColumn: "MaVatTu",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietDonHang",
                columns: table => new
                {
                    MaCTDH = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDH = table.Column<int>(type: "int", nullable: false),
                    MaVatTu = table.Column<int>(type: "int", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    ThanhTien = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietDonHang", x => x.MaCTDH);
                    table.ForeignKey(
                        name: "FK_ChiTietDonHang_DonHang_MaDH",
                        column: x => x.MaDH,
                        principalTable: "DonHang",
                        principalColumn: "MaDH",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietDonHang_VatTu_MaVatTu",
                        column: x => x.MaVatTu,
                        principalTable: "VatTu",
                        principalColumn: "MaVatTu",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHang_MaDH",
                table: "ChiTietDonHang",
                column: "MaDH");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHang_MaVatTu",
                table: "ChiTietDonHang",
                column: "MaVatTu");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietNhapKho_MaNK",
                table: "ChiTietNhapKho",
                column: "MaNK");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietNhapKho_MaVatTu",
                table: "ChiTietNhapKho",
                column: "MaVatTu");

            migrationBuilder.CreateIndex(
                name: "IX_DonHang_MaKH",
                table: "DonHang",
                column: "MaKH");

            migrationBuilder.CreateIndex(
                name: "IX_DonHang_SoDH",
                table: "DonHang",
                column: "SoDH",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KhachHang_MaKHCode",
                table: "KhachHang",
                column: "MaKHCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VatTu_MaHang",
                table: "VatTu",
                column: "MaHang",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VatTu_MaLoai",
                table: "VatTu",
                column: "MaLoai");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietDonHang");

            migrationBuilder.DropTable(
                name: "ChiTietNhapKho");

            migrationBuilder.DropTable(
                name: "ThanhVien");

            migrationBuilder.DropTable(
                name: "DonHang");

            migrationBuilder.DropTable(
                name: "NhapKho");

            migrationBuilder.DropTable(
                name: "VatTu");

            migrationBuilder.DropTable(
                name: "KhachHang");

            migrationBuilder.DropTable(
                name: "DanhMucLoai");
        }
    }
}
