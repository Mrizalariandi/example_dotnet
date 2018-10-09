using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace api_bakum.Migrations
{
    public partial class _2018Juni19 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "BantuanHukum",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "KodeLaporan",
                table: "BantuanHukum",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "LastStatusID",
                table: "BantuanHukum",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LogEmails",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Subject = table.Column<string>(nullable: true),
                    To = table.Column<string>(nullable: true),
                    Body = table.Column<string>(nullable: true),
                    Sent = table.Column<bool>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Error = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogEmails", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "StatusBantuan",
                columns: table => new
                {
                    ID = table.Column<short>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusBantuan", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Templates",
                columns: table => new
                {
                    ID = table.Column<short>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TemplateName = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Templates", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Conversations",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    HeaderID = table.Column<long>(nullable: true),
                    StatusID = table.Column<short>(nullable: true),
                    ParentIDID = table.Column<long>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    IsRead = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversations", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Conversations_BantuanHukum_HeaderID",
                        column: x => x.HeaderID,
                        principalTable: "BantuanHukum",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Conversations_Conversations_ParentIDID",
                        column: x => x.ParentIDID,
                        principalTable: "Conversations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Conversations_StatusBantuan_StatusID",
                        column: x => x.StatusID,
                        principalTable: "StatusBantuan",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BantuanHukum_LastStatusID",
                table: "BantuanHukum",
                column: "LastStatusID");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_HeaderID",
                table: "Conversations",
                column: "HeaderID");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_ParentIDID",
                table: "Conversations",
                column: "ParentIDID");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_StatusID",
                table: "Conversations",
                column: "StatusID");

            migrationBuilder.AddForeignKey(
                name: "FK_BantuanHukum_StatusBantuan_LastStatusID",
                table: "BantuanHukum",
                column: "LastStatusID",
                principalTable: "StatusBantuan",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BantuanHukum_StatusBantuan_LastStatusID",
                table: "BantuanHukum");

            migrationBuilder.DropTable(
                name: "Conversations");

            migrationBuilder.DropTable(
                name: "LogEmails");

            migrationBuilder.DropTable(
                name: "Templates");

            migrationBuilder.DropTable(
                name: "StatusBantuan");

            migrationBuilder.DropIndex(
                name: "IX_BantuanHukum_LastStatusID",
                table: "BantuanHukum");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "BantuanHukum");

            migrationBuilder.DropColumn(
                name: "KodeLaporan",
                table: "BantuanHukum");

            migrationBuilder.DropColumn(
                name: "LastStatusID",
                table: "BantuanHukum");
        }
    }
}
