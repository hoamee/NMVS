using Microsoft.EntityFrameworkCore.Migrations;

namespace NMVS.Migrations
{
    public partial class AddShpDetPackCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PackCount",
                table: "ShipperDets",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PackCount",
                table: "ShipperDets");
        }
    }
}
