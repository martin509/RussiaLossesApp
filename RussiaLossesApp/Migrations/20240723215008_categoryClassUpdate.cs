using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RussiaLossesApp.Migrations
{
    /// <inheritdoc />
    public partial class categoryClassUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "categoryClass",
                table: "EquipCategory",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "categoryClass",
                table: "EquipCategory");
        }
    }
}
