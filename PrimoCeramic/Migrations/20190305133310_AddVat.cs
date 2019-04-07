using Microsoft.EntityFrameworkCore.Migrations;

namespace PrimoCeramic.Migrations
{
    public partial class AddVat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Vat",
                table: "ProductsSelectedForOrder",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Vat",
                table: "Orders",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Vat",
                table: "ProductsSelectedForOrder");

            migrationBuilder.DropColumn(
                name: "Vat",
                table: "Orders");
        }
    }
}
