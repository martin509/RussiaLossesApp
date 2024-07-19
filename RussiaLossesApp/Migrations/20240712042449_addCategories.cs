using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RussiaLossesApp.Migrations
{
    /// <inheritdoc />
    public partial class addCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EquipCategoryName",
                table: "EquipType",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EquipCategory",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipCategory", x => x.Name);
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquipType_EquipCategory_EquipCategoryName",
                table: "EquipType");

            migrationBuilder.DropTable(
                name: "EquipCategory");

            migrationBuilder.DropIndex(
                name: "IX_EquipType_EquipCategoryName",
                table: "EquipType");

            migrationBuilder.DropColumn(
                name: "EquipCategoryName",
                table: "EquipType");
        }
    }
}
