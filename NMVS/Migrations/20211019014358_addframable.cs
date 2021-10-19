using Microsoft.EntityFrameworkCore.Migrations;

namespace NMVS.Migrations
{
    public partial class addframable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PkgType",
                table: "Locs");

            migrationBuilder.AddColumn<bool>(
                name: "Flammable",
                table: "Locs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Flammable",
                table: "ItemDatas",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Flammable",
                table: "Locs");

            migrationBuilder.DropColumn(
                name: "Flammable",
                table: "ItemDatas");

            migrationBuilder.AddColumn<string>(
                name: "PkgType",
                table: "Locs",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
