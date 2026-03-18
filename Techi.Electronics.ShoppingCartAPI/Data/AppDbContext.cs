using Microsoft.EntityFrameworkCore;
using Techi.Electronics.ShoppingCartAPI.Models;

namespace Techi.Electronics.ShoppingCartAPI.Data
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<CartHeader> CartHeaders { get; set; }
        public DbSet<CartDetails> CartDetails { get; set; }
    }
}
