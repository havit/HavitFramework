using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Havit.EFCoreTests.Entity.Migrations
{
    /// <inheritdoc />
    public partial class Migration_Person_Deleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Deleted",
                table: "Person",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Symbol",
                table: "Language",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "Symbol",
                table: "Language");
        }
    }
}
