using Microsoft.EntityFrameworkCore.Migrations;

namespace api_bakum.Migrations
{
    public partial class patchFKConversation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_BantuanHukum_HeaderID",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_Conversations_ParentIDID",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_StatusBantuan_StatusID",
                table: "Conversations");

            migrationBuilder.DropIndex(
                name: "IX_BantuanHukum_StatusBantuanForeignKey",
                table: "BantuanHukum");

            migrationBuilder.RenameColumn(
                name: "StatusID",
                table: "Conversations",
                newName: "StatusBantuanForeignKey");

            migrationBuilder.RenameColumn(
                name: "ParentIDID",
                table: "Conversations",
                newName: "ParentIDForeignKey");

            migrationBuilder.RenameColumn(
                name: "HeaderID",
                table: "Conversations",
                newName: "HeaderLaporanForeignKey");

            migrationBuilder.RenameIndex(
                name: "IX_Conversations_StatusID",
                table: "Conversations",
                newName: "IX_Conversations_StatusBantuanForeignKey");

            migrationBuilder.RenameIndex(
                name: "IX_Conversations_ParentIDID",
                table: "Conversations",
                newName: "IX_Conversations_ParentIDForeignKey");

            migrationBuilder.RenameIndex(
                name: "IX_Conversations_HeaderID",
                table: "Conversations",
                newName: "IX_Conversations_HeaderLaporanForeignKey");

            migrationBuilder.CreateIndex(
                name: "IX_BantuanHukum_StatusBantuanForeignKey",
                table: "BantuanHukum",
                column: "StatusBantuanForeignKey");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_BantuanHukum_HeaderLaporanForeignKey",
                table: "Conversations",
                column: "HeaderLaporanForeignKey",
                principalTable: "BantuanHukum",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_Conversations_ParentIDForeignKey",
                table: "Conversations",
                column: "ParentIDForeignKey",
                principalTable: "Conversations",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_StatusBantuan_StatusBantuanForeignKey",
                table: "Conversations",
                column: "StatusBantuanForeignKey",
                principalTable: "StatusBantuan",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_BantuanHukum_HeaderLaporanForeignKey",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_Conversations_ParentIDForeignKey",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_StatusBantuan_StatusBantuanForeignKey",
                table: "Conversations");

            migrationBuilder.DropIndex(
                name: "IX_BantuanHukum_StatusBantuanForeignKey",
                table: "BantuanHukum");

            migrationBuilder.RenameColumn(
                name: "StatusBantuanForeignKey",
                table: "Conversations",
                newName: "StatusID");

            migrationBuilder.RenameColumn(
                name: "ParentIDForeignKey",
                table: "Conversations",
                newName: "ParentIDID");

            migrationBuilder.RenameColumn(
                name: "HeaderLaporanForeignKey",
                table: "Conversations",
                newName: "HeaderID");

            migrationBuilder.RenameIndex(
                name: "IX_Conversations_StatusBantuanForeignKey",
                table: "Conversations",
                newName: "IX_Conversations_StatusID");

            migrationBuilder.RenameIndex(
                name: "IX_Conversations_ParentIDForeignKey",
                table: "Conversations",
                newName: "IX_Conversations_ParentIDID");

            migrationBuilder.RenameIndex(
                name: "IX_Conversations_HeaderLaporanForeignKey",
                table: "Conversations",
                newName: "IX_Conversations_HeaderID");

            migrationBuilder.CreateIndex(
                name: "IX_BantuanHukum_StatusBantuanForeignKey",
                table: "BantuanHukum",
                column: "StatusBantuanForeignKey",
                unique: true,
                filter: "[StatusBantuanForeignKey] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_BantuanHukum_HeaderID",
                table: "Conversations",
                column: "HeaderID",
                principalTable: "BantuanHukum",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_Conversations_ParentIDID",
                table: "Conversations",
                column: "ParentIDID",
                principalTable: "Conversations",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_StatusBantuan_StatusID",
                table: "Conversations",
                column: "StatusID",
                principalTable: "StatusBantuan",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
