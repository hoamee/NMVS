using Microsoft.EntityFrameworkCore.Migrations;

namespace NMVS.Migrations
{
    public partial class AddSiteAttriBute : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Site",
                table: "WorkOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Site",
                table: "Shippers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Site",
                table: "RequestDets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Site",
                table: "ItemMasters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Site",
                table: "IssueOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Site",
                table: "IncomingLists",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Site",
                table: "AllocateRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Site",
                table: "AllocateOrders",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Site",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "Site",
                table: "Shippers");

            migrationBuilder.DropColumn(
                name: "Site",
                table: "RequestDets");

            migrationBuilder.DropColumn(
                name: "Site",
                table: "ItemMasters");

            migrationBuilder.DropColumn(
                name: "Site",
                table: "IssueOrders");

            migrationBuilder.DropColumn(
                name: "Site",
                table: "IncomingLists");

            migrationBuilder.DropColumn(
                name: "Site",
                table: "AllocateRequests");

            migrationBuilder.DropColumn(
                name: "Site",
                table: "AllocateOrders");
        }
    }
}
