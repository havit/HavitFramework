using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Havit.EFCoreTests.Entity.Migrations
{
    /// <inheritdoc />
    public partial class StateAndLocalization2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StateLocalization_ParentId",
                table: "StateLocalization");

            migrationBuilder.CreateIndex(
                name: "IX_StateLocalization_ParentId_LanguageId",
                table: "StateLocalization",
                columns: new[] { "ParentId", "LanguageId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StateLocalization_ParentId_LanguageId",
                table: "StateLocalization");

            migrationBuilder.CreateIndex(
                name: "IX_StateLocalization_ParentId",
                table: "StateLocalization",
                column: "ParentId");
        }
    }
}
