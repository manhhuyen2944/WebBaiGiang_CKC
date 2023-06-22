using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBaiGiang_CKC.Migrations
{
    public partial class baigiangckc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BaiLam",
                columns: table => new
                {
                    BaiLamId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MSSV = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HoTen = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoCauDung = table.Column<int>(type: "int", nullable: true),
                    Diem = table.Column<float>(type: "real", nullable: true),
                    ThoiGianBatDau = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ThoiGianDenHan = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaiLam", x => x.BaiLamId);
                });

            migrationBuilder.CreateTable(
                name: "GiaoVien",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDangNhap = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AnhDaiDien = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsGiaoVien = table.Column<bool>(type: "bit", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiaoVien", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KyKiemTra",
                columns: table => new
                {
                    KyKiemTraId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenKyKiemTra = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThoiGianBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThoiGianKetThuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThoiGianLamBai = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KyKiemTra", x => x.KyKiemTraId);
                });

            migrationBuilder.CreateTable(
                name: "MonHoc",
                columns: table => new
                {
                    MonHocId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenMonHoc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaMonHoc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTa = table.Column<string>(type: "ntext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonHoc", x => x.MonHocId);
                });

            migrationBuilder.CreateTable(
                name: "TaiKhoan",
                columns: table => new
                {
                    TaiKhoanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MSSV = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HoTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiKhoan", x => x.TaiKhoanId);
                });

            migrationBuilder.CreateTable(
                name: "De",
                columns: table => new
                {
                    DeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KyKiemTraId = table.Column<int>(type: "int", nullable: false),
                    SoCauHoi = table.Column<int>(type: "int", nullable: false),
                    DoKhoDe = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_De", x => x.DeId);
                    table.ForeignKey(
                        name: "FK_De_KyKiemTra_KyKiemTraId",
                        column: x => x.KyKiemTraId,
                        principalTable: "KyKiemTra",
                        principalColumn: "KyKiemTraId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Chuong",
                columns: table => new
                {
                    ChuongId = table.Column<int>(type: "int", nullable: false),
                    TenChuong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MonHocId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chuong", x => x.ChuongId);
                    table.ForeignKey(
                        name: "FK_Chuong_MonHoc_MonHocId",
                        column: x => x.MonHocId,
                        principalTable: "MonHoc",
                        principalColumn: "MonHocId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DanhSachThi",
                columns: table => new
                {
                    DanhSachThiId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaiKhoanId = table.Column<int>(type: "int", nullable: false),
                    KyKiemTraId = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhSachThi", x => x.DanhSachThiId);
                    table.ForeignKey(
                        name: "FK_DanhSachThi_KyKiemTra_KyKiemTraId",
                        column: x => x.KyKiemTraId,
                        principalTable: "KyKiemTra",
                        principalColumn: "KyKiemTraId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DanhSachThi_TaiKhoan_TaiKhoanId",
                        column: x => x.TaiKhoanId,
                        principalTable: "TaiKhoan",
                        principalColumn: "TaiKhoanId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bai",
                columns: table => new
                {
                    BaiId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChuongId = table.Column<int>(type: "int", nullable: false),
                    TenBai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoBai = table.Column<int>(type: "int", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bai", x => x.BaiId);
                    table.ForeignKey(
                        name: "FK_Bai_Chuong_ChuongId",
                        column: x => x.ChuongId,
                        principalTable: "Chuong",
                        principalColumn: "ChuongId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CauHoi",
                columns: table => new
                {
                    CauHoiId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChuongId = table.Column<int>(type: "int", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DapAnA = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DapAnB = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DapAnC = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DapAnD = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DapAnDung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DoKho = table.Column<float>(type: "real", nullable: false),
                    SoLanLay = table.Column<int>(type: "int", nullable: true),
                    SoLanTraLoiDung = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CauHoi", x => x.CauHoiId);
                    table.ForeignKey(
                        name: "FK_CauHoi_Chuong_ChuongId",
                        column: x => x.ChuongId,
                        principalTable: "Chuong",
                        principalColumn: "ChuongId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Muc",
                columns: table => new
                {
                    MucId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenMuc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BaiId = table.Column<int>(type: "int", nullable: false),
                    MucSo = table.Column<int>(type: "int", nullable: false),
                    NoiDung = table.Column<string>(type: "ntext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Muc", x => x.MucId);
                    table.ForeignKey(
                        name: "FK_Muc_Bai_BaiId",
                        column: x => x.BaiId,
                        principalTable: "Bai",
                        principalColumn: "BaiId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CauHoi_De",
                columns: table => new
                {
                    CauHoi_DeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CauHoiId = table.Column<int>(type: "int", nullable: false),
                    DeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CauHoi_De", x => x.CauHoi_DeId);
                    table.ForeignKey(
                        name: "FK_CauHoi_De_CauHoi_CauHoiId",
                        column: x => x.CauHoiId,
                        principalTable: "CauHoi",
                        principalColumn: "CauHoiId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CauHoi_De_De_DeId",
                        column: x => x.DeId,
                        principalTable: "De",
                        principalColumn: "DeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CauHoi_BaiLam",
                columns: table => new
                {
                    CauHoi_BaiLamId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BaiLamId = table.Column<int>(type: "int", nullable: false),
                    CauHoi_DeId = table.Column<int>(type: "int", nullable: false),
                    DapAnSVChon = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CauHoi_BaiLam", x => x.CauHoi_BaiLamId);
                    table.ForeignKey(
                        name: "FK_CauHoi_BaiLam_BaiLam_BaiLamId",
                        column: x => x.BaiLamId,
                        principalTable: "BaiLam",
                        principalColumn: "BaiLamId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CauHoi_BaiLam_CauHoi_De_CauHoi_DeId",
                        column: x => x.CauHoi_DeId,
                        principalTable: "CauHoi_De",
                        principalColumn: "CauHoi_DeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bai_ChuongId",
                table: "Bai",
                column: "ChuongId");

            migrationBuilder.CreateIndex(
                name: "IX_CauHoi_ChuongId",
                table: "CauHoi",
                column: "ChuongId");

            migrationBuilder.CreateIndex(
                name: "IX_CauHoi_BaiLam_BaiLamId",
                table: "CauHoi_BaiLam",
                column: "BaiLamId");

            migrationBuilder.CreateIndex(
                name: "IX_CauHoi_BaiLam_CauHoi_DeId",
                table: "CauHoi_BaiLam",
                column: "CauHoi_DeId");

            migrationBuilder.CreateIndex(
                name: "IX_CauHoi_De_CauHoiId",
                table: "CauHoi_De",
                column: "CauHoiId");

            migrationBuilder.CreateIndex(
                name: "IX_CauHoi_De_DeId",
                table: "CauHoi_De",
                column: "DeId");

            migrationBuilder.CreateIndex(
                name: "IX_Chuong_ChuongId",
                table: "Chuong",
                column: "ChuongId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chuong_MonHocId",
                table: "Chuong",
                column: "MonHocId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhSachThi_KyKiemTraId",
                table: "DanhSachThi",
                column: "KyKiemTraId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhSachThi_TaiKhoanId",
                table: "DanhSachThi",
                column: "TaiKhoanId");

            migrationBuilder.CreateIndex(
                name: "IX_De_KyKiemTraId",
                table: "De",
                column: "KyKiemTraId");

            migrationBuilder.CreateIndex(
                name: "IX_Muc_BaiId",
                table: "Muc",
                column: "BaiId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CauHoi_BaiLam");

            migrationBuilder.DropTable(
                name: "DanhSachThi");

            migrationBuilder.DropTable(
                name: "GiaoVien");

            migrationBuilder.DropTable(
                name: "Muc");

            migrationBuilder.DropTable(
                name: "BaiLam");

            migrationBuilder.DropTable(
                name: "CauHoi_De");

            migrationBuilder.DropTable(
                name: "TaiKhoan");

            migrationBuilder.DropTable(
                name: "Bai");

            migrationBuilder.DropTable(
                name: "CauHoi");

            migrationBuilder.DropTable(
                name: "De");

            migrationBuilder.DropTable(
                name: "Chuong");

            migrationBuilder.DropTable(
                name: "KyKiemTra");

            migrationBuilder.DropTable(
                name: "MonHoc");
        }
    }
}
