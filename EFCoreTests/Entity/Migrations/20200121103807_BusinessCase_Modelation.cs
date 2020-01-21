using Microsoft.EntityFrameworkCore.Migrations;

namespace Havit.EFCoreTests.Entity.Migrations
{
    public partial class BusinessCase_Modelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BusinessCase",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessCase", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Modelation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BusinessCaseId = table.Column<int>(nullable: true),
                    ModelationId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modelation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Modelation_BusinessCase_BusinessCaseId",
                        column: x => x.BusinessCaseId,
                        principalTable: "BusinessCase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Modelation_BusinessCaseId",
                table: "Modelation",
                column: "BusinessCaseId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Modelation");

            migrationBuilder.DropTable(
                name: "BusinessCase");
        }
    }
}
