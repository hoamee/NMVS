using Microsoft.EntityFrameworkCore.Migrations;

namespace NMVS.Migrations
{
    public partial class shipperDetSoldTo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShipTo",
                table: "ShipperDets",
                newName: "SoldToName");

            migrationBuilder.AddColumn<string>(
                name: "ShipToId",
                table: "ShipperDets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShipToName",
                table: "ShipperDets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SoldTo",
                table: "ShipperDets",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShipToId",
                table: "ShipperDets");

            migrationBuilder.DropColumn(
                name: "ShipToName",
                table: "ShipperDets");

            migrationBuilder.DropColumn(
                name: "SoldTo",
                table: "ShipperDets");

            migrationBuilder.RenameColumn(
                name: "SoldToName",
                table: "ShipperDets",
                newName: "ShipTo");
        }
    }
}
