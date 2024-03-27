using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Havit.EFCoreTests.Entity.Migrations;

/// <inheritdoc />
public partial class StateAndLocalization : Migration
{
	/// <inheritdoc />
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.CreateTable(
			name: "Language",
			columns: table => new
			{
				Id = table.Column<int>(type: "int", nullable: false)
					.Annotation("SqlServer:Identity", "1, 1"),
				Culture = table.Column<string>(type: "nvarchar(max)", nullable: true),
				UiCulture = table.Column<string>(type: "nvarchar(max)", nullable: true)
			},
			constraints: table =>
			{
				table.PrimaryKey("PK_Language", x => x.Id);
			});

		migrationBuilder.CreateTable(
			name: "State",
			columns: table => new
			{
				Id = table.Column<int>(type: "int", nullable: false)
					.Annotation("SqlServer:Identity", "1, 1")
			},
			constraints: table =>
			{
				table.PrimaryKey("PK_State", x => x.Id);
			});

		migrationBuilder.CreateTable(
			name: "StateLocalization",
			columns: table => new
			{
				Id = table.Column<int>(type: "int", nullable: false)
					.Annotation("SqlServer:Identity", "1, 1"),
				ParentId = table.Column<int>(type: "int", nullable: false),
				LanguageId = table.Column<int>(type: "int", nullable: false)
			},
			constraints: table =>
			{
				table.PrimaryKey("PK_StateLocalization", x => x.Id);
				table.ForeignKey(
					name: "FK_StateLocalization_Language_LanguageId",
					column: x => x.LanguageId,
					principalTable: "Language",
					principalColumn: "Id",
					onDelete: ReferentialAction.Restrict);
				table.ForeignKey(
					name: "FK_StateLocalization_State_ParentId",
					column: x => x.ParentId,
					principalTable: "State",
					principalColumn: "Id",
					onDelete: ReferentialAction.Restrict);
			});

		migrationBuilder.CreateIndex(
			name: "IX_StateLocalization_LanguageId",
			table: "StateLocalization",
			column: "LanguageId");

		migrationBuilder.CreateIndex(
			name: "IX_StateLocalization_ParentId",
			table: "StateLocalization",
			column: "ParentId");
	}

	/// <inheritdoc />
	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropTable(
			name: "StateLocalization");

		migrationBuilder.DropTable(
			name: "Language");

		migrationBuilder.DropTable(
			name: "State");
	}
}