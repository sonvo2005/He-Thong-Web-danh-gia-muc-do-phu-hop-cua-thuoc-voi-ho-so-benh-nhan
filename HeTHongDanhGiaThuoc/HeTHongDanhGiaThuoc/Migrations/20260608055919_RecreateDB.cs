using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HeTHongDanhGiaThuoc.Migrations
{
    /// <inheritdoc />
    public partial class RecreateDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HoatChat",
                columns: table => new
                {
                    MaHoatChat = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenHoatChat = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoatChat", x => x.MaHoatChat);
                });

            migrationBuilder.CreateTable(
                name: "Thuoc",
                columns: table => new
                {
                    MaThuoc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenThuoc = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TenBietDuoc = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DangBaoChe = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LieuDungChuan = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TinhTrangThuoc = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MoTaChiTiet = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Thuoc", x => x.MaThuoc);
                });

            migrationBuilder.CreateTable(
                name: "VaiTro",
                columns: table => new
                {
                    MaVaiTro = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenVaiTro = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaiTro", x => x.MaVaiTro);
                });

            migrationBuilder.CreateTable(
                name: "ChongChiDinh",
                columns: table => new
                {
                    MaChongChiDinh = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaThuoc = table.Column<int>(type: "int", nullable: false),
                    TenBenhChongChiDinh = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChongChiDinh", x => x.MaChongChiDinh);
                    table.ForeignKey(
                        name: "FK_ChongChiDinh_Thuoc_MaThuoc",
                        column: x => x.MaThuoc,
                        principalTable: "Thuoc",
                        principalColumn: "MaThuoc",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Thuoc_HoatChat",
                columns: table => new
                {
                    MaThuocHoatChat = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaThuoc = table.Column<int>(type: "int", nullable: false),
                    MaHoatChat = table.Column<int>(type: "int", nullable: false),
                    HamLuong = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Thuoc_HoatChat", x => x.MaThuocHoatChat);
                    table.ForeignKey(
                        name: "FK_Thuoc_HoatChat_HoatChat_MaHoatChat",
                        column: x => x.MaHoatChat,
                        principalTable: "HoatChat",
                        principalColumn: "MaHoatChat",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Thuoc_HoatChat_Thuoc_MaThuoc",
                        column: x => x.MaThuoc,
                        principalTable: "Thuoc",
                        principalColumn: "MaThuoc",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TuongTacThuoc",
                columns: table => new
                {
                    MaTuongTac = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaThuoc_A = table.Column<int>(type: "int", nullable: false),
                    MaThuoc_B = table.Column<int>(type: "int", nullable: false),
                    MucDoNghiemTrong = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MoTaTuongTac = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TuongTacThuoc", x => x.MaTuongTac);
                    table.ForeignKey(
                        name: "FK_TuongTacThuoc_Thuoc_MaThuoc_A",
                        column: x => x.MaThuoc_A,
                        principalTable: "Thuoc",
                        principalColumn: "MaThuoc",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TuongTacThuoc_Thuoc_MaThuoc_B",
                        column: x => x.MaThuoc_B,
                        principalTable: "Thuoc",
                        principalColumn: "MaThuoc",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NguoiDung",
                columns: table => new
                {
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDangNhap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MatKhauMaHoa = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SoDienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    MaVaiTro = table.Column<int>(type: "int", nullable: false),
                    TrangThaiHoatDong = table.Column<bool>(type: "bit", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguoiDung", x => x.MaNguoiDung);
                    table.ForeignKey(
                        name: "FK_NguoiDung_VaiTro_MaVaiTro",
                        column: x => x.MaVaiTro,
                        principalTable: "VaiTro",
                        principalColumn: "MaVaiTro",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HoSoBenhNhan",
                columns: table => new
                {
                    MaBenhNhan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDinhDanhBenhNhan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GioiTinh = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    MaNguoiTao = table.Column<int>(type: "int", nullable: true),
                    NgayTiepNhan = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSoBenhNhan", x => x.MaBenhNhan);
                    table.ForeignKey(
                        name: "FK_HoSoBenhNhan_NguoiDung_MaNguoiTao",
                        column: x => x.MaNguoiTao,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "BenhNen",
                columns: table => new
                {
                    MaBenhNen = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaBenhNhan = table.Column<int>(type: "int", nullable: true),
                    TenBenhNen = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    NgayChanDoan = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BenhNen", x => x.MaBenhNen);
                    table.ForeignKey(
                        name: "FK_BenhNen_HoSoBenhNhan_MaBenhNhan",
                        column: x => x.MaBenhNhan,
                        principalTable: "HoSoBenhNhan",
                        principalColumn: "MaBenhNhan",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DanhGia",
                columns: table => new
                {
                    MaDanhGia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaBenhNhan = table.Column<int>(type: "int", nullable: true),
                    MaNguoiDanhGia = table.Column<int>(type: "int", nullable: true),
                    NgayDanhGia = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TyLePhuHop = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CanhBaoYTe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KhuyenNghiSuDung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChuPhienDanhGia = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhGia", x => x.MaDanhGia);
                    table.ForeignKey(
                        name: "FK_DanhGia_HoSoBenhNhan_MaBenhNhan",
                        column: x => x.MaBenhNhan,
                        principalTable: "HoSoBenhNhan",
                        principalColumn: "MaBenhNhan",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DanhGia_NguoiDung_MaNguoiDanhGia",
                        column: x => x.MaNguoiDanhGia,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "DiUngThuoc",
                columns: table => new
                {
                    MaDiUng = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaBenhNhan = table.Column<int>(type: "int", nullable: true),
                    MaHoatChat = table.Column<int>(type: "int", nullable: true),
                    MucDoDiUng = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiUngThuoc", x => x.MaDiUng);
                    table.ForeignKey(
                        name: "FK_DiUngThuoc_HoSoBenhNhan_MaBenhNhan",
                        column: x => x.MaBenhNhan,
                        principalTable: "HoSoBenhNhan",
                        principalColumn: "MaBenhNhan",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiUngThuoc_HoatChat_MaHoatChat",
                        column: x => x.MaHoatChat,
                        principalTable: "HoatChat",
                        principalColumn: "MaHoatChat",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietDanhGia",
                columns: table => new
                {
                    MaChiTietDanhGia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDanhGia = table.Column<int>(type: "int", nullable: false),
                    MaThuoc = table.Column<int>(type: "int", nullable: false),
                    LieuDungChiDinh = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CoPhuHopKhong = table.Column<bool>(type: "bit", nullable: false),
                    LyDoKhongPhuHop = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietDanhGia", x => x.MaChiTietDanhGia);
                    table.ForeignKey(
                        name: "FK_ChiTietDanhGia_DanhGia_MaDanhGia",
                        column: x => x.MaDanhGia,
                        principalTable: "DanhGia",
                        principalColumn: "MaDanhGia",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietDanhGia_Thuoc_MaThuoc",
                        column: x => x.MaThuoc,
                        principalTable: "Thuoc",
                        principalColumn: "MaThuoc",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "VaiTro",
                columns: new[] { "MaVaiTro", "TenVaiTro" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "Bác sĩ" },
                    { 3, "Dược sĩ" },
                    { 4, "Y tá" }
                });

            migrationBuilder.InsertData(
                table: "NguoiDung",
                columns: new[] { "MaNguoiDung", "Email", "HoTen", "MaVaiTro", "MatKhauMaHoa", "NgayTao", "SoDienThoai", "TenDangNhap", "TrangThaiHoatDong" },
                values: new object[] { 1, "admin@hethong.com", "Quản trị viên", 1, "$2a$11$LwHiYJeSfXcTwon7BtHoPuo2mmuMNv3Mt4ku1gl075l63vWBk3mWm", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "admin", true });

            migrationBuilder.CreateIndex(
                name: "IX_BenhNen_MaBenhNhan",
                table: "BenhNen",
                column: "MaBenhNhan");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDanhGia_MaDanhGia",
                table: "ChiTietDanhGia",
                column: "MaDanhGia");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDanhGia_MaThuoc",
                table: "ChiTietDanhGia",
                column: "MaThuoc");

            migrationBuilder.CreateIndex(
                name: "IX_ChongChiDinh_MaThuoc",
                table: "ChongChiDinh",
                column: "MaThuoc");

            migrationBuilder.CreateIndex(
                name: "IX_DanhGia_MaBenhNhan",
                table: "DanhGia",
                column: "MaBenhNhan");

            migrationBuilder.CreateIndex(
                name: "IX_DanhGia_MaNguoiDanhGia",
                table: "DanhGia",
                column: "MaNguoiDanhGia");

            migrationBuilder.CreateIndex(
                name: "IX_DiUngThuoc_MaBenhNhan",
                table: "DiUngThuoc",
                column: "MaBenhNhan");

            migrationBuilder.CreateIndex(
                name: "IX_DiUngThuoc_MaHoatChat",
                table: "DiUngThuoc",
                column: "MaHoatChat");

            migrationBuilder.CreateIndex(
                name: "IX_HoatChat_TenHoatChat",
                table: "HoatChat",
                column: "TenHoatChat",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HoSoBenhNhan_MaDinhDanhBenhNhan",
                table: "HoSoBenhNhan",
                column: "MaDinhDanhBenhNhan",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HoSoBenhNhan_MaNguoiTao",
                table: "HoSoBenhNhan",
                column: "MaNguoiTao");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDung_MaVaiTro",
                table: "NguoiDung",
                column: "MaVaiTro");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDung_TenDangNhap",
                table: "NguoiDung",
                column: "TenDangNhap",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Thuoc_HoatChat_MaHoatChat",
                table: "Thuoc_HoatChat",
                column: "MaHoatChat");

            migrationBuilder.CreateIndex(
                name: "IX_Thuoc_HoatChat_MaThuoc",
                table: "Thuoc_HoatChat",
                column: "MaThuoc");

            migrationBuilder.CreateIndex(
                name: "IX_TuongTacThuoc_MaThuoc_A",
                table: "TuongTacThuoc",
                column: "MaThuoc_A");

            migrationBuilder.CreateIndex(
                name: "IX_TuongTacThuoc_MaThuoc_B",
                table: "TuongTacThuoc",
                column: "MaThuoc_B");

            migrationBuilder.CreateIndex(
                name: "IX_VaiTro_TenVaiTro",
                table: "VaiTro",
                column: "TenVaiTro",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BenhNen");

            migrationBuilder.DropTable(
                name: "ChiTietDanhGia");

            migrationBuilder.DropTable(
                name: "ChongChiDinh");

            migrationBuilder.DropTable(
                name: "DiUngThuoc");

            migrationBuilder.DropTable(
                name: "Thuoc_HoatChat");

            migrationBuilder.DropTable(
                name: "TuongTacThuoc");

            migrationBuilder.DropTable(
                name: "DanhGia");

            migrationBuilder.DropTable(
                name: "HoatChat");

            migrationBuilder.DropTable(
                name: "Thuoc");

            migrationBuilder.DropTable(
                name: "HoSoBenhNhan");

            migrationBuilder.DropTable(
                name: "NguoiDung");

            migrationBuilder.DropTable(
                name: "VaiTro");
        }
    }
}
