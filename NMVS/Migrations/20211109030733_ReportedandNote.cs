using Microsoft.EntityFrameworkCore.Migrations;

namespace NMVS.Migrations
{
    public partial class ReportedandNote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ReqReported",
                table: "SalesOrders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ReqReportedNote",
                table: "SalesOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Reported",
                table: "InvRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ReportedNote",
                table: "InvRequests",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReqReported",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "ReqReportedNote",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "Reported",
                table: "InvRequests");

            migrationBuilder.DropColumn(
                name: "ReportedNote",
                table: "InvRequests");
        }
    }
}
