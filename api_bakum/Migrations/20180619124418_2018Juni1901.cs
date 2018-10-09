using Microsoft.EntityFrameworkCore.Migrations;

namespace api_bakum.Migrations
{
    public partial class _2018Juni1901 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BantuanHukum_StatusBantuan_LastStatusID",
                table: "BantuanHukum");

            migrationBuilder.DropIndex(
                name: "IX_BantuanHukum_LastStatusID",
                table: "BantuanHukum");

            migrationBuilder.RenameColumn(
                name: "LastStatusID",
                table: "BantuanHukum",
                newName: "StatusBantuanForeignKey");

            migrationBuilder.CreateIndex(
                name: "IX_BantuanHukum_StatusBantuanForeignKey",
                table: "BantuanHukum",
                column: "StatusBantuanForeignKey",
                unique: true,
                filter: "[StatusBantuanForeignKey] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_BantuanHukum_StatusBantuan_StatusBantuanForeignKey",
                table: "BantuanHukum",
                column: "StatusBantuanForeignKey",
                principalTable: "StatusBantuan",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BantuanHukum_StatusBantuan_StatusBantuanForeignKey",
                table: "BantuanHukum");

            migrationBuilder.DropIndex(
                name: "IX_BantuanHukum_StatusBantuanForeignKey",
                table: "BantuanHukum");

            migrationBuilder.RenameColumn(
                name: "StatusBantuanForeignKey",
                table: "BantuanHukum",
                newName: "LastStatusID");

            migrationBuilder.CreateIndex(
                name: "IX_BantuanHukum_LastStatusID",
                table: "BantuanHukum",
                column: "LastStatusID");

            migrationBuilder.AddForeignKey(
                name: "FK_BantuanHukum_StatusBantuan_LastStatusID",
                table: "BantuanHukum",
                column: "LastStatusID",
                principalTable: "StatusBantuan",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
