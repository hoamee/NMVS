using Microsoft.EntityFrameworkCore.Migrations;

namespace NMVS.Migrations
{
    public partial class sodetRqDet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Avail",
                table: "SoDetails");

            migrationBuilder.AddColumn<int>(
                name: "RqDetId",
                table: "SoDetails",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RqDetId",
                table: "SoDetails");

            migrationBuilder.AddColumn<double>(
                name: "Avail",
                table: "SoDetails",
                type: "float",
                nullable: true);
        }
    }
}
