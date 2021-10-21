using Microsoft.EntityFrameworkCore.Migrations;

namespace NMVS.Migrations
{
    public partial class changePtMstrIdToInt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "PtId",
                table: "ItemMasters",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemMasters",
                table: "ItemMasters",
                column: "PtId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemMasters",
                table: "ItemMasters",
                column: "ItemNo");
        }
    }
}
