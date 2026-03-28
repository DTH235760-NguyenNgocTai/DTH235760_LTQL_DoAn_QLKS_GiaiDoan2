using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLKS.Migrations
{
    /// <inheritdoc />
    public partial class ThemIsDeleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TrangThaiPhong",
                table: "Phongs",
                newName: "TrangThai");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Phongs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Phongs");

            migrationBuilder.RenameColumn(
                name: "TrangThai",
                table: "Phongs",
                newName: "TrangThaiPhong");
        }
    }
}
