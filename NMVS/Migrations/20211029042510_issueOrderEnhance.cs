using Microsoft.EntityFrameworkCore.Migrations;

namespace NMVS.Migrations
{
    public partial class issueOrderEnhance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IssueOrders_Locs_LocCode",
                table: "IssueOrders");

            migrationBuilder.DropIndex(
                name: "IX_IssueOrders_LocCode",
                table: "IssueOrders");

            migrationBuilder.DropColumn(
                name: "ToLocDesc",
                table: "IssueOrders");

            migrationBuilder.RenameColumn(
                name: "WhlCode",
                table: "IssueOrders",
                newName: "IssueToDesc");

            migrationBuilder.AlterColumn<string>(
                name: "LocCode",
                table: "IssueOrders",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ToVehicle",
                table: "IssueOrders",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ToVehicle",
                table: "IssueOrders");

            migrationBuilder.RenameColumn(
                name: "IssueToDesc",
                table: "IssueOrders",
                newName: "WhlCode");

            migrationBuilder.AlterColumn<string>(
                name: "LocCode",
                table: "IssueOrders",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ToLocDesc",
                table: "IssueOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IssueOrders_LocCode",
                table: "IssueOrders",
                column: "LocCode");

            migrationBuilder.AddForeignKey(
                name: "FK_IssueOrders_Locs_LocCode",
                table: "IssueOrders",
                column: "LocCode",
                principalTable: "Locs",
                principalColumn: "LocCode",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
