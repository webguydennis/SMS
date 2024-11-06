using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TextGateKeeper.Migrations
{
    /// <inheritdoc />
    public partial class RenameTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_textMessages",
                table: "textMessages");

            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.RenameTable(
                name: "textMessages",
                newName: "TextMessage",
                newSchema: "dbo");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TextMessage",
                schema: "dbo",
                table: "TextMessage",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TextMessage",
                schema: "dbo",
                table: "TextMessage");

            migrationBuilder.RenameTable(
                name: "TextMessage",
                schema: "dbo",
                newName: "textMessages");

            migrationBuilder.AddPrimaryKey(
                name: "PK_textMessages",
                table: "textMessages",
                column: "Id");
        }
    }
}
