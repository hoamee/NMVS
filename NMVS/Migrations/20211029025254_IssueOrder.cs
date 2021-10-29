using Microsoft.EntityFrameworkCore.Migrations;

namespace NMVS.Migrations
{
    public partial class IssueOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IssueOrders_InvRequests_InvRequestRqID",
                table: "IssueOrders");

            migrationBuilder.DropIndex(
                name: "IX_IssueOrders_InvRequestRqID",
                table: "IssueOrders");

            migrationBuilder.DropColumn(
                name: "InvRequestRqID",
                table: "IssueOrders");

            migrationBuilder.AddColumn<int>(
                name: "DetId",
                table: "IssueOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DetId",
                table: "IssueOrders");

            migrationBuilder.AddColumn<string>(
                name: "InvRequestRqID",
                table: "IssueOrders",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IssueOrders_InvRequestRqID",
                table: "IssueOrders",
                column: "InvRequestRqID");

            migrationBuilder.AddForeignKey(
                name: "FK_IssueOrders_InvRequests_InvRequestRqID",
                table: "IssueOrders",
                column: "InvRequestRqID",
                principalTable: "InvRequests",
                principalColumn: "RqID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
