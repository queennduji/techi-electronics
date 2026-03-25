using Microsoft.EntityFrameworkCore;
using Techi.Electronics.RewardAPI.Models;

namespace Techi.Electronics.RewardAPI.Data
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Rewards> Rewards { get; set; }


    }
}
