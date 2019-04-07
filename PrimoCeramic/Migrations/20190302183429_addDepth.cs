using Microsoft.EntityFrameworkCore.Migrations;

namespace PrimoCeramic.Migrations
{
    public partial class addDepth : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Depth",
                table: "Products",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Depth",
                table: "Products");
        }
    }
}
