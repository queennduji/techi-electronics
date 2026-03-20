using Microsoft.EntityFrameworkCore;
using Techi.Electronics.EmailAPI.Data.Model;

namespace Techi.Electronics.EmailAPI.Data
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<EmailLogger> EmailLoggers { get; set; }


    }
}
