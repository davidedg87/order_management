using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhotoSiTest.OrderService.Data.Migrations
{
    /// <inheritdoc />
    public partial class OrderConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId_AddressId_OrderDate",
                table: "Orders",
                columns: new[] { "UserId", "AddressId", "OrderDate" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_UserId_AddressId_OrderDate",
                table: "Orders");
        }
    }
}
