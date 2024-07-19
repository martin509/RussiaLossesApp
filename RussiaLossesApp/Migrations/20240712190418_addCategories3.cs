using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RussiaLossesApp.Migrations
{
    /// <inheritdoc />
    public partial class addCategories3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquipType_EquipCategory_EquipCategoryName",
                table: "EquipType");

            migrationBuilder.DropIndex(
                name: "IX_EquipType_EquipCategoryName",
                table: "EquipType");

            migrationBuilder.DropColumn(
                name: "EquipCategoryName",
                table: "EquipType");

            migrationBuilder.CreateTable(
                name: "EquipCategoryEquipType",
                columns: table => new
                {
                    EquipCategoriesName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EquipTypesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipCategoryEquipType", x => new { x.EquipCategoriesName, x.EquipTypesId });
                    table.ForeignKey(
                        name: "FK_EquipCategoryEquipType_EquipCategory_EquipCategoriesName",
                        column: x => x.EquipCategoriesName,
                        principalTable: "EquipCategory",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EquipCategoryEquipType_EquipType_EquipTypesId",
                        column: x => x.EquipTypesId,
                        principalTable: "EquipType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EquipCategoryEquipType_EquipTypesId",
                table: "EquipCategoryEquipType",
                column: "EquipTypesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EquipCategoryEquipType");

            migrationBuilder.AddColumn<string>(
                name: "EquipCategoryName",
                table: "EquipType",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EquipType_EquipCategoryName",
                table: "EquipType",
                column: "EquipCategoryName");

            migrationBuilder.AddForeignKey(
                name: "FK_EquipType_EquipCategory_EquipCategoryName",
                table: "EquipType",
                column: "EquipCategoryName",
                principalTable: "EquipCategory",
                principalColumn: "Name");
        }
    }
}
