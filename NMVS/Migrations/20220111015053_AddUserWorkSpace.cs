using Microsoft.EntityFrameworkCore.Migrations;

namespace NMVS.Migrations
{
    public partial class AddUserWorkSpace : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WorkSpace",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorkSpace",
                table: "AspNetUsers");
        }
    }
}
