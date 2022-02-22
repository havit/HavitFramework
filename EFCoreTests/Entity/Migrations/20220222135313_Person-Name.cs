using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Havit.EFCoreTests.Entity.Migrations
{
    public partial class PersonName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Person",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Person");
        }
    }
}
