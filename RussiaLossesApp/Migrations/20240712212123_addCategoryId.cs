using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RussiaLossesApp.Migrations
{
    /// <inheritdoc />
    public partial class addCategoryId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquipCategoryEquipType_EquipCategory_EquipCategoriesName",
                table: "EquipCategoryEquipType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EquipCategoryEquipType",
                table: "EquipCategoryEquipType");

            migrationBuilder.DropIndex(
                name: "IX_EquipCategoryEquipType_EquipTypesId",
                table: "EquipCategoryEquipType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EquipCategory",
                table: "EquipCategory");

            migrationBuilder.DropColumn(
                name: "EquipCategoriesName",
                table: "EquipCategoryEquipType");

            migrationBuilder.AddColumn<int>(
                name: "equipCategoriesId",
                table: "EquipCategoryEquipType",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "EquipCategory",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "EquipCategory",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EquipCategoryEquipType",
                table: "EquipCategoryEquipType",
                columns: new[] { "EquipTypesId", "equipCategoriesId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_EquipCategory",
                table: "EquipCategory",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_EquipCategoryEquipType_equipCategoriesId",
                table: "EquipCategoryEquipType",
                column: "equipCategoriesId");

            migrationBuilder.AddForeignKey(
                name: "FK_EquipCategoryEquipType_EquipCategory_equipCategoriesId",
                table: "EquipCategoryEquipType",
                column: "equipCategoriesId",
                principalTable: "EquipCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquipCategoryEquipType_EquipCategory_equipCategoriesId",
                table: "EquipCategoryEquipType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EquipCategoryEquipType",
                table: "EquipCategoryEquipType");

            migrationBuilder.DropIndex(
                name: "IX_EquipCategoryEquipType_equipCategoriesId",
                table: "EquipCategoryEquipType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EquipCategory",
                table: "EquipCategory");

            migrationBuilder.DropColumn(
                name: "equipCategoriesId",
                table: "EquipCategoryEquipType");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "EquipCategory");

            migrationBuilder.AddColumn<string>(
                name: "EquipCategoriesName",
                table: "EquipCategoryEquipType",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "EquipCategory",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EquipCategoryEquipType",
                table: "EquipCategoryEquipType",
                columns: new[] { "EquipCategoriesName", "EquipTypesId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_EquipCategory",
                table: "EquipCategory",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_EquipCategoryEquipType_EquipTypesId",
                table: "EquipCategoryEquipType",
                column: "EquipTypesId");

            migrationBuilder.AddForeignKey(
                name: "FK_EquipCategoryEquipType_EquipCategory_EquipCategoriesName",
                table: "EquipCategoryEquipType",
                column: "EquipCategoriesName",
                principalTable: "EquipCategory",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
