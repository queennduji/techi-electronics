using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Techi.Electronics.ProductAPI.Migrations
{
    /// <inheritdoc />
    public partial class addProductAndSeedTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "CategoryName", "Description", "ImageUrl", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "Smartphones", "Apple iPhone 15 Pro with A17 Pro chip, 128GB storage, 6.1-inch Super Retina XDR display, and advanced triple-camera system.", "https://placehold.co/603x403", "Apple iPhone 15 Pro", 999.99000000000001 },
                    { 2, "Smartphones", "Samsung Galaxy S24 Ultra with Snapdragon processor, 200MP camera system, S-Pen support, and 6.8-inch Dynamic AMOLED display.", "https://placehold.co/602x402", "Samsung Galaxy S24 Ultra", 1199.99 },
                    { 3, "Audio", "Premium wireless headphones with industry-leading noise cancellation, up to 30 hours battery life, and crystal-clear audio.", "https://placehold.co/601x401", "Sony WH-1000XM5 Noise Cancelling Headphones", 399.99000000000001 },
                    { 4, "Laptops", "Dell XPS 13 ultrabook with Intel Core i7 processor, 16GB RAM, 512GB SSD, and stunning InfinityEdge display.", "https://placehold.co/600x400", "Dell XPS 13 Laptop", 1299.99 },
                    { 5, "Wearables", "Apple Watch Series 9 with advanced health monitoring, fitness tracking, always-on Retina display, and seamless iPhone integration.", "https://placehold.co/600x401", "Apple Watch Series 9", 399.99000000000001 },
                    { 6, "Accessories", "Advanced wireless mouse with ultra-fast scrolling, precision tracking, and customizable buttons designed for productivity.", "https://placehold.co/600x402", "Logitech MX Master 3S Wireless Mouse", 99.989999999999995 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
