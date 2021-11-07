using Microsoft.EntityFrameworkCore.Migrations;

namespace NMVS.Migrations
{
    public partial class InvTransac1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PtId",
                table: "InventoryTransacs");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PtId",
                table: "InventoryTransacs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
