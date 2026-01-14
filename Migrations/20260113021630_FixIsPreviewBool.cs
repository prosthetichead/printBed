using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrintBed.Migrations
{
    /// <inheritdoc />
    public partial class FixIsPreviewBool : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPreivew",
                table: "PrintFile");

            migrationBuilder.AddColumn<bool>(
                name: "IsPreview",
                table: "PrintFile",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPreview",
                table: "PrintFile");

            migrationBuilder.AddColumn<bool>(
                name: "IsPreivew",
                table: "PrintFile",
                type: "INTEGER",
                nullable: true);
        }
    }
}
