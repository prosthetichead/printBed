using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PrintBed.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    ImagePath = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Creator",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    ImagePath = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Creator", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileType",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Extensions = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    ImagePath = table.Column<string>(type: "TEXT", nullable: true),
                    PreviewType = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tag",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tag", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Print",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    CategoryId = table.Column<string>(type: "TEXT", nullable: false),
                    CreatorId = table.Column<string>(type: "TEXT", nullable: false),
                    PrintInstructions = table.Column<string>(type: "TEXT", nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModified = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TagString = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Print", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Print_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Print_Creator_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Creator",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrintFile",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    FileName = table.Column<string>(type: "TEXT", nullable: false),
                    FileExtension = table.Column<string>(type: "TEXT", nullable: false),
                    FilePath = table.Column<string>(type: "TEXT", nullable: false),
                    FileSize = table.Column<double>(type: "REAL", nullable: false),
                    FileTypeId = table.Column<string>(type: "TEXT", nullable: true),
                    PrintId = table.Column<string>(type: "TEXT", nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModified = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrintFile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrintFile_FileType_FileTypeId",
                        column: x => x.FileTypeId,
                        principalTable: "FileType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PrintFile_Print_PrintId",
                        column: x => x.PrintId,
                        principalTable: "Print",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PrintTag",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    PrintId = table.Column<string>(type: "TEXT", nullable: true),
                    TagId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrintTag", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrintTag_Print_PrintId",
                        column: x => x.PrintId,
                        principalTable: "Print",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PrintTag_Tag_TagId",
                        column: x => x.TagId,
                        principalTable: "Tag",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Category",
                columns: new[] { "Id", "ImagePath", "Name" },
                values: new object[,]
                {
                    { "0", "/img/uncategorised.png", "Uncategorised" },
                    { "100", "/img/miniature.png", "Miniatures" },
                    { "200", "/img/statue.png", "Statues" }
                });

            migrationBuilder.InsertData(
                table: "Creator",
                columns: new[] { "Id", "ImagePath", "Name" },
                values: new object[,]
                {
                    { "0", "/img/unknown-creator.png", "Unknown" },
                    { "100", "/img/epic-miniatures.png", "Epic Miniatures" },
                    { "200", "/img/loot-studios.png", "Loot Studios" },
                    { "300", "/img/titan-forge.png", "Titan Forge" }
                });

            migrationBuilder.InsertData(
                table: "FileType",
                columns: new[] { "Id", "Extensions", "ImagePath", "Name", "PreviewType" },
                values: new object[,]
                {
                    { "0", "", "/img/unknown.png", "Unknown File Type", "No Preview" },
                    { "100", "stl,obj", "/img/cube.png", "Model", "Model Viewer" },
                    { "200", "lys,chitubox", "/img/slicer.png", "Slicer Project", "No Preview" },
                    { "300", "gcode,goo", "/img/print.png", "Printer Code", "No Preview" },
                    { "400", "jpg,png,webp", "/img/image.png", "Image", "Image" },
                    { "500", "pdf,docx,txt,md", "/img/doc.png", "Document", "No Preview" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Print_CategoryId",
                table: "Print",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Print_CreatorId",
                table: "Print",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_PrintFile_FileTypeId",
                table: "PrintFile",
                column: "FileTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PrintFile_PrintId",
                table: "PrintFile",
                column: "PrintId");

            migrationBuilder.CreateIndex(
                name: "IX_PrintTag_PrintId",
                table: "PrintTag",
                column: "PrintId");

            migrationBuilder.CreateIndex(
                name: "IX_PrintTag_TagId",
                table: "PrintTag",
                column: "TagId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrintFile");

            migrationBuilder.DropTable(
                name: "PrintTag");

            migrationBuilder.DropTable(
                name: "FileType");

            migrationBuilder.DropTable(
                name: "Print");

            migrationBuilder.DropTable(
                name: "Tag");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "Creator");
        }
    }
}
