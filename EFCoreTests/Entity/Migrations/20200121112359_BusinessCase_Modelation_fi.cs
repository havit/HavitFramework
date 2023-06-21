using Microsoft.EntityFrameworkCore.Migrations;

namespace Havit.EFCoreTests.Entity.Migrations;

public partial class BusinessCase_Modelation_fi : Migration
{
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropColumn(
			name: "ModelationId",
			table: "Modelation");

		migrationBuilder.AlterColumn<int>(
			name: "BusinessCaseId",
			table: "Modelation",
			nullable: false,
			oldClrType: typeof(int),
			oldType: "int",
			oldNullable: true);
	}

	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.AlterColumn<int>(
			name: "BusinessCaseId",
			table: "Modelation",
			type: "int",
			nullable: true,
			oldClrType: typeof(int));

		migrationBuilder.AddColumn<int>(
			name: "ModelationId",
			table: "Modelation",
			type: "int",
			nullable: false,
			defaultValue: 0);
	}
}
