using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhotoSiTest.ProductService.Data.Migrations
{
    /// <inheritdoc />
    public partial class ProductConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Products_Name_ProductCategoryId",
                table: "Products",
                columns: new[] { "Name", "ProductCategoryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_Name_Description",
                table: "ProductCategories",
                columns: new[] { "Name", "Description" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_Name_ProductCategoryId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_ProductCategories_Name_Description",
                table: "ProductCategories");
        }
    }
}
