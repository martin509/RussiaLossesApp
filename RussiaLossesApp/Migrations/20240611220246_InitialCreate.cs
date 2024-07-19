using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RussiaLossesApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EquipTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                    category = table.Column<string>(nullable: false),
                    name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipTypes", x => x.Id);
                }
            );

            migrationBuilder.AddColumn<int>(
                name: "EquipTypeId",
                table: "LossObject",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(@"
                INSERT INTO EquipTypes (Name, Category)
                SELECT DISTINCT model, type FROM LossObject;
    
                UPDATE LossObject
                SET EquipTypeId = EquipTypes.Id
                FROM LossObject
                INNER JOIN EquipTypes ON LossObject.model = EquipTypes.Name AND LossObject.type = EquipTypes.Category;
             ");
            migrationBuilder.DropColumn(
                name: "type",
                table: "LossObject");
            migrationBuilder.DropColumn(
                name: "model",
                table: "LossObject");
            /*migrationBuilder.CreateTable(
                name: "LossObject",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    //type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    //model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    lost_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    nearest_location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    geo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    unit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    tags = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LossObject", x => x.Id);
                });*/

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LossObject");
        }
    }
}
