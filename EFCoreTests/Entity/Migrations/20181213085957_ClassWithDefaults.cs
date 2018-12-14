using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Havit.EFCoreTests.Entity.Migrations
{
    public partial class ClassWithDefaults : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClassWithDefaults",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateTimeValue = table.Column<DateTime>(nullable: false, defaultValue: new DateTime(2018, 12, 24, 0, 0, 0, 0, DateTimeKind.Unspecified)),
                    IntValue = table.Column<int>(nullable: false, defaultValue: 0),
                    StringValue = table.Column<string>(nullable: true, defaultValue: "ABC"),
                    BoolValue = table.Column<bool>(nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassWithDefaults", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassWithDefaults");
        }
    }
}
