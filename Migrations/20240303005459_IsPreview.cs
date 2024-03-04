using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrintBed.Migrations
{
    /// <inheritdoc />
    public partial class IsPreview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPreivew",
                table: "PrintFile",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPreivew",
                table: "PrintFile");
        }
    }
}
