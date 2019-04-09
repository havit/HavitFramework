using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Havit.EFCoreTests.Entity.Migrations
{
    public partial class AutoStav : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Barva",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Barva", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stav",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stav", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Auto",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BarvaId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Auto_Barva_BarvaId",
                        column: x => x.BarvaId,
                        principalTable: "Barva",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StavLocalization",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ParentId = table.Column<int>(nullable: false),
                    LanguageId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StavLocalization", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StavLocalization_Language_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StavLocalization_Stav_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Stav",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Auto_BarvaId",
                table: "Auto",
                column: "BarvaId");

            migrationBuilder.CreateIndex(
                name: "IX_StavLocalization_LanguageId",
                table: "StavLocalization",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_StavLocalization_ParentId",
                table: "StavLocalization",
                column: "ParentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Auto");

            migrationBuilder.DropTable(
                name: "StavLocalization");

            migrationBuilder.DropTable(
                name: "Barva");

            migrationBuilder.DropTable(
                name: "Stav");
        }
    }
}
