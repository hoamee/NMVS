using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NMVS.Migrations
{
    public partial class changePtMstrId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemMasters",
                table: "ItemMasters");

            migrationBuilder.DropColumn(
                name: "PtId",
                table: "ItemMasters");

            migrationBuilder.AlterColumn<string>(
                name: "ItemNo",
                table: "ItemMasters",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "PoDate",
                table: "IncomingLists",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemMasters",
                table: "ItemMasters",
                column: "ItemNo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemMasters",
                table: "ItemMasters");

            migrationBuilder.AlterColumn<string>(
                name: "ItemNo",
                table: "ItemMasters",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "PtId",
                table: "ItemMasters",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PoDate",
                table: "IncomingLists",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemMasters",
                table: "ItemMasters",
                column: "PtId");
        }
    }
}
