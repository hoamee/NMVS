using Microsoft.EntityFrameworkCore.Migrations;

namespace NMVS.Migrations
{
    public partial class LinkUnqualifiedToSoNbr : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SoNbr",
                table: "Unqualifieds",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SoNbr",
                table: "Unqualifieds");
        }
    }
}
