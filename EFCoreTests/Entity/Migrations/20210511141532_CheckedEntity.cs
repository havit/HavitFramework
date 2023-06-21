using Microsoft.EntityFrameworkCore.Migrations;

namespace Havit.EFCoreTests.Entity.Migrations;

public partial class CheckedEntity : Migration
{
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.CreateTable(
			name: "CheckedEntity",
			columns: table => new
			{
				Id = table.Column<int>(type: "int", nullable: false)
					.Annotation("SqlServer:Identity", "1, 1"),
				Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
				Version = table.Column<int>(type: "int", nullable: false)
			},
			constraints: table =>
			{
				table.PrimaryKey("PK_CheckedEntity", x => x.Id);
			});
	}

	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropTable(
			name: "CheckedEntity");
	}
}
