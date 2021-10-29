using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NMVS.Migrations
{
    public partial class check : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestDets_ItemDatas_ItemDataItemNo",
                table: "RequestDets");

            migrationBuilder.DropIndex(
                name: "IX_RequestDets_ItemDataItemNo",
                table: "RequestDets");

            migrationBuilder.DropColumn(
                name: "ItemDataItemNo",
                table: "RequestDets");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RequireDate",
                table: "RequestDets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "RequireDate",
                table: "RequestDets",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "ItemDataItemNo",
                table: "RequestDets",
                type: "nvarchar(50)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RequestDets_ItemDataItemNo",
                table: "RequestDets",
                column: "ItemDataItemNo");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestDets_ItemDatas_ItemDataItemNo",
                table: "RequestDets",
                column: "ItemDataItemNo",
                principalTable: "ItemDatas",
                principalColumn: "ItemNo",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
