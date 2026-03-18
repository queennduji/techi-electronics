using Microsoft.EntityFrameworkCore;
using Techi.Electronics.CouponAPI.Models;

namespace Techi.Electronics.CouponAPI.Data
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Coupon> Coupons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Coupon>().HasData(
                new Coupon
                {
                    CouponId = 1,
                    CouponCode = "SAVE10",
                    DiscountAmount = 10,
                    MinAmount = 100
                },
                new Coupon
                {
                    CouponId = 2,
                    CouponCode = "TECH20",
                    DiscountAmount = 20,
                    MinAmount = 200
                },
                new Coupon
                {
                    CouponId = 3,
                    CouponCode = "FREESHIP",
                    DiscountAmount = 15,
                    MinAmount = 75
                },
                new Coupon
                {
                    CouponId = 4,
                    CouponCode = "BLACKFRIDAY",
                    DiscountAmount = 50,
                    MinAmount = 500
                },
                new Coupon
                {
                    CouponId = 5,
                    CouponCode = "NEWUSER25",
                    DiscountAmount = 25,
                    MinAmount = 150
                }
            );
        }
    }
}
