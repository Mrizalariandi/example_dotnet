using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace api_bakum.Migrations
{
    public partial class MenuRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Menus",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: true),
                    FKParentMenu = table.Column<int>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    IsMobile = table.Column<bool>(nullable: false),
                    UISRef = table.Column<string>(nullable: true),
                    Icon = table.Column<string>(nullable: true),
                    Css = table.Column<string>(nullable: true),
                    IsFolder = table.Column<bool>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Menus_Menus_FKParentMenu",
                        column: x => x.FKParentMenu,
                        principalTable: "Menus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MenuRoles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FKParentMenu = table.Column<int>(nullable: true),
                    FKRole = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuRoles_Menus_FKParentMenu",
                        column: x => x.FKParentMenu,
                        principalTable: "Menus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MenuRoles_AspNetRoles_FKRole",
                        column: x => x.FKRole,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MenuRoles_FKParentMenu",
                table: "MenuRoles",
                column: "FKParentMenu");

            migrationBuilder.CreateIndex(
                name: "IX_MenuRoles_FKRole",
                table: "MenuRoles",
                column: "FKRole");

            migrationBuilder.CreateIndex(
                name: "IX_Menus_FKParentMenu",
                table: "Menus",
                column: "FKParentMenu");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MenuRoles");

            migrationBuilder.DropTable(
                name: "Menus");
        }
    }
}
