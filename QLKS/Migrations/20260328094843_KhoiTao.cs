using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLKS.Migrations
{
    /// <inheritdoc />
    public partial class KhoiTao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LoaiPhongs",
                columns: table => new
                {
                    LoaiPhongId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenLoaiPhong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GiaCoBan = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SoNguoiToiDa = table.Column<int>(type: "int", nullable: false),
                    DienTich = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoaiPhongs", x => x.LoaiPhongId);
                });

            migrationBuilder.CreateTable(
                name: "Tangs",
                columns: table => new
                {
                    TangId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoTang = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tangs", x => x.TangId);
                });

            migrationBuilder.CreateTable(
                name: "ThietBis",
                columns: table => new
                {
                    ThietBiId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenThietBi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DonViTinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GiaTri = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThietBis", x => x.ThietBiId);
                });

            migrationBuilder.CreateTable(
                name: "TienNghis",
                columns: table => new
                {
                    TienNghiId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenTienNghi = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TienNghis", x => x.TienNghiId);
                });

            migrationBuilder.CreateTable(
                name: "Phongs",
                columns: table => new
                {
                    PhongId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoPhong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoaiPhongId = table.Column<int>(type: "int", nullable: false),
                    TangId = table.Column<int>(type: "int", nullable: false),
                    TrangThaiPhong = table.Column<int>(type: "int", nullable: false),
                    HinhAnh = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Phongs", x => x.PhongId);
                    table.ForeignKey(
                        name: "FK_Phongs_LoaiPhongs_LoaiPhongId",
                        column: x => x.LoaiPhongId,
                        principalTable: "LoaiPhongs",
                        principalColumn: "LoaiPhongId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Phongs_Tangs_TangId",
                        column: x => x.TangId,
                        principalTable: "Tangs",
                        principalColumn: "TangId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoaiPhongThietBis",
                columns: table => new
                {
                    LoaiPhongThietBiId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoaiPhongId = table.Column<int>(type: "int", nullable: false),
                    ThietBiId = table.Column<int>(type: "int", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoaiPhongThietBis", x => x.LoaiPhongThietBiId);
                    table.ForeignKey(
                        name: "FK_LoaiPhongThietBis_LoaiPhongs_LoaiPhongId",
                        column: x => x.LoaiPhongId,
                        principalTable: "LoaiPhongs",
                        principalColumn: "LoaiPhongId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LoaiPhongThietBis_ThietBis_ThietBiId",
                        column: x => x.ThietBiId,
                        principalTable: "ThietBis",
                        principalColumn: "ThietBiId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoaiPhongTienNghis",
                columns: table => new
                {
                    LoaiPhongTienNghiId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoaiPhongId = table.Column<int>(type: "int", nullable: false),
                    TienNghiId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoaiPhongTienNghis", x => x.LoaiPhongTienNghiId);
                    table.ForeignKey(
                        name: "FK_LoaiPhongTienNghis_LoaiPhongs_LoaiPhongId",
                        column: x => x.LoaiPhongId,
                        principalTable: "LoaiPhongs",
                        principalColumn: "LoaiPhongId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LoaiPhongTienNghis_TienNghis_TienNghiId",
                        column: x => x.TienNghiId,
                        principalTable: "TienNghis",
                        principalColumn: "TienNghiId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LoaiPhongThietBis_LoaiPhongId",
                table: "LoaiPhongThietBis",
                column: "LoaiPhongId");

            migrationBuilder.CreateIndex(
                name: "IX_LoaiPhongThietBis_ThietBiId",
                table: "LoaiPhongThietBis",
                column: "ThietBiId");

            migrationBuilder.CreateIndex(
                name: "IX_LoaiPhongTienNghis_LoaiPhongId",
                table: "LoaiPhongTienNghis",
                column: "LoaiPhongId");

            migrationBuilder.CreateIndex(
                name: "IX_LoaiPhongTienNghis_TienNghiId",
                table: "LoaiPhongTienNghis",
                column: "TienNghiId");

            migrationBuilder.CreateIndex(
                name: "IX_Phongs_LoaiPhongId",
                table: "Phongs",
                column: "LoaiPhongId");

            migrationBuilder.CreateIndex(
                name: "IX_Phongs_TangId",
                table: "Phongs",
                column: "TangId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoaiPhongThietBis");

            migrationBuilder.DropTable(
                name: "LoaiPhongTienNghis");

            migrationBuilder.DropTable(
                name: "Phongs");

            migrationBuilder.DropTable(
                name: "ThietBis");

            migrationBuilder.DropTable(
                name: "TienNghis");

            migrationBuilder.DropTable(
                name: "LoaiPhongs");

            migrationBuilder.DropTable(
                name: "Tangs");
        }
    }
}
