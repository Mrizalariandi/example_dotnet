using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace api_bakum.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    CountryID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.CountryID);
                });

            migrationBuilder.CreateTable(
                name: "BantuanHukum",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NamaLengkap = table.Column<string>(nullable: true),
                    Umur = table.Column<int>(nullable: false),
                    Pekerjaan = table.Column<string>(nullable: true),
                    JenisKelamin = table.Column<string>(nullable: true),
                    Agama = table.Column<string>(nullable: true),
                    KewarganegaraanCountryID = table.Column<int>(nullable: true),
                    Telpn = table.Column<string>(nullable: true),
                    Identitas = table.Column<string>(nullable: true),
                    NoIdentitas = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    IsiPermohonan = table.Column<string>(nullable: true),
                    FileName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BantuanHukum", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BantuanHukum_Countries_KewarganegaraanCountryID",
                        column: x => x.KewarganegaraanCountryID,
                        principalTable: "Countries",
                        principalColumn: "CountryID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BantuanHukum_KewarganegaraanCountryID",
                table: "BantuanHukum",
                column: "KewarganegaraanCountryID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BantuanHukum");

            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
