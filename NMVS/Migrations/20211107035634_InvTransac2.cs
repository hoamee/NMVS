using Microsoft.EntityFrameworkCore.Migrations;

namespace NMVS.Migrations
{
    public partial class InvTransac2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MovementType",
                table: "InventoryTransacs");

            migrationBuilder.AddColumn<bool>(
                name: "IsAllocate",
                table: "InventoryTransacs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDisposed",
                table: "InventoryTransacs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAllocate",
                table: "InventoryTransacs");

            migrationBuilder.DropColumn(
                name: "IsDisposed",
                table: "InventoryTransacs");

            migrationBuilder.AddColumn<int>(
                name: "MovementType",
                table: "InventoryTransacs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
