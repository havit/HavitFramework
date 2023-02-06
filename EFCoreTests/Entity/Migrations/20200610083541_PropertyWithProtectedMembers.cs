using Microsoft.EntityFrameworkCore.Migrations;

namespace Havit.EFCoreTests.Entity.Migrations
{
	public partial class PropertyWithProtectedMembers : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "PropertyWithProtectedMembers",
				columns: table => new
				{
					Id = table.Column<int>(nullable: false),
					ProtectedSetterValue = table.Column<string>(nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_PropertyWithProtectedMembers", x => x.Id);
				});
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "PropertyWithProtectedMembers");
		}
	}
}
