using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Havit.EFCoreTests.Entity.Migrations
{
	/// <inheritdoc />
	public partial class Migration_User_Guid : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable("User", "dbo");

			migrationBuilder.CreateTable(
				name: "User",
				columns: table => new
				{
					Id = table.Column<Guid>(nullable: false),
					Username = table.Column<string>(nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_User", x => x.Id);
				});
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			throw new NotSupportedException("This migration cannot be reverted.");
		}
	}
}