using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceGraduation.Infrastrucutre.Migrations
{
    /// <inheritdoc />
    public partial class AddedShippingCostColumnToShippingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ShippingCost",
                table: "Shipping",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShippingCost",
                table: "Shipping");
        }
    }
}
