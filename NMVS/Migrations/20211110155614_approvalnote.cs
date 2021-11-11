using Microsoft.EntityFrameworkCore.Migrations;

namespace NMVS.Migrations
{
    public partial class approvalnote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApprovalNote",
                table: "SalesOrders",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovalNote",
                table: "SalesOrders");
        }
    }
}
