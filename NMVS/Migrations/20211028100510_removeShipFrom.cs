using Microsoft.EntityFrameworkCore.Migrations;

namespace NMVS.Migrations
{
    public partial class removeShipFrom : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShpFrom",
                table: "Shippers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShpFrom",
                table: "Shippers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
