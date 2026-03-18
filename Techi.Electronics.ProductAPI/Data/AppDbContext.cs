using Microsoft.EntityFrameworkCore;
using Techi.Electronics.ProductAPI.Models;

namespace Techi.Electronics.ProductAPI.Data
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>().HasData(new Product
            {
                ProductId = 1,
                Name = "Apple iPhone 15 Pro",
                Price = 999.99,
                Description = "Apple iPhone 15 Pro with A17 Pro chip, 128GB storage, 6.1-inch Super Retina XDR display, and advanced triple-camera system.",
                ImageUrl = "https://placehold.co/603x403",
                CategoryName = "Smartphones"
            });

            modelBuilder.Entity<Product>().HasData(new Product
            {
                ProductId = 2,
                Name = "Samsung Galaxy S24 Ultra",
                Price = 1199.99,
                Description = "Samsung Galaxy S24 Ultra with Snapdragon processor, 200MP camera system, S-Pen support, and 6.8-inch Dynamic AMOLED display.",
                ImageUrl = "https://placehold.co/602x402",
                CategoryName = "Smartphones"
            });

            modelBuilder.Entity<Product>().HasData(new Product
            {
                ProductId = 3,
                Name = "Sony WH-1000XM5 Noise Cancelling Headphones",
                Price = 399.99,
                Description = "Premium wireless headphones with industry-leading noise cancellation, up to 30 hours battery life, and crystal-clear audio.",
                ImageUrl = "https://placehold.co/601x401",
                CategoryName = "Audio"
            });

            modelBuilder.Entity<Product>().HasData(new Product
            {
                ProductId = 4,
                Name = "Dell XPS 13 Laptop",
                Price = 1299.99,
                Description = "Dell XPS 13 ultrabook with Intel Core i7 processor, 16GB RAM, 512GB SSD, and stunning InfinityEdge display.",
                ImageUrl = "https://placehold.co/600x400",
                CategoryName = "Laptops"
            });

            modelBuilder.Entity<Product>().HasData(new Product
            {
                ProductId = 5,
                Name = "Apple Watch Series 9",
                Price = 399.99,
                Description = "Apple Watch Series 9 with advanced health monitoring, fitness tracking, always-on Retina display, and seamless iPhone integration.",
                ImageUrl = "https://placehold.co/600x401",
                CategoryName = "Wearables"
            });

            modelBuilder.Entity<Product>().HasData(new Product
            {
                ProductId = 6,
                Name = "Logitech MX Master 3S Wireless Mouse",
                Price = 99.99,
                Description = "Advanced wireless mouse with ultra-fast scrolling, precision tracking, and customizable buttons designed for productivity.",
                ImageUrl = "https://placehold.co/600x402",
                CategoryName = "Accessories"
            });
        }
    }
}
