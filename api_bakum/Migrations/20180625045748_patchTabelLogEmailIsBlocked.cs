using Microsoft.EntityFrameworkCore.Migrations;

namespace api_bakum.Migrations
{
    public partial class patchTabelLogEmailIsBlocked : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Blocked",
                table: "LogEmails",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Blocked",
                table: "LogEmails");
        }
    }
}
