using Microsoft.EntityFrameworkCore.Migrations;

namespace NMVS.Migrations
{
    public partial class refresh : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvRequests_GeneralizedCodes_GeneralizedCodeCodeNo",
                table: "InvRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestDets_InvRequests_InvRequestRqID",
                table: "RequestDets");

            migrationBuilder.DropIndex(
                name: "IX_RequestDets_InvRequestRqID",
                table: "RequestDets");

            migrationBuilder.DropIndex(
                name: "IX_InvRequests_GeneralizedCodeCodeNo",
                table: "InvRequests");

            migrationBuilder.DropColumn(
                name: "InvRequestRqID",
                table: "RequestDets");

            migrationBuilder.DropColumn(
                name: "GeneralizedCodeCodeNo",
                table: "InvRequests");

            migrationBuilder.RenameColumn(
                name: "CodeValue",
                table: "InvRequests",
                newName: "RqType");

            migrationBuilder.AlterColumn<string>(
                name: "Ref",
                table: "InvRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RqType",
                table: "InvRequests",
                newName: "CodeValue");

            migrationBuilder.AddColumn<string>(
                name: "InvRequestRqID",
                table: "RequestDets",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Ref",
                table: "InvRequests",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "GeneralizedCodeCodeNo",
                table: "InvRequests",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RequestDets_InvRequestRqID",
                table: "RequestDets",
                column: "InvRequestRqID");

            migrationBuilder.CreateIndex(
                name: "IX_InvRequests_GeneralizedCodeCodeNo",
                table: "InvRequests",
                column: "GeneralizedCodeCodeNo");

            migrationBuilder.AddForeignKey(
                name: "FK_InvRequests_GeneralizedCodes_GeneralizedCodeCodeNo",
                table: "InvRequests",
                column: "GeneralizedCodeCodeNo",
                principalTable: "GeneralizedCodes",
                principalColumn: "CodeNo",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestDets_InvRequests_InvRequestRqID",
                table: "RequestDets",
                column: "InvRequestRqID",
                principalTable: "InvRequests",
                principalColumn: "RqID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
