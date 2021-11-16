using Microsoft.EntityFrameworkCore.Migrations;

namespace NMVS.Migrations
{
    public partial class UseOnlyOneIssueNoteDet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WrIssueNoteDets");

            migrationBuilder.DropTable(
                name: "WtIssueNoteDets");

            migrationBuilder.AddColumn<int>(
                name: "InType",
                table: "SoIssueNoteDets",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InType",
                table: "SoIssueNoteDets");

            migrationBuilder.CreateTable(
                name: "WrIssueNoteDets",
                columns: table => new
                {
                    IndID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InId = table.Column<int>(type: "int", nullable: false),
                    ItemNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PackCount = table.Column<int>(type: "int", nullable: false),
                    PtId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WrIssueNoteDets", x => x.IndID);
                });

            migrationBuilder.CreateTable(
                name: "WtIssueNoteDets",
                columns: table => new
                {
                    IndID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InId = table.Column<int>(type: "int", nullable: false),
                    ItemNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PackCount = table.Column<int>(type: "int", nullable: false),
                    PtId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WtIssueNoteDets", x => x.IndID);
                });
        }
    }
}
