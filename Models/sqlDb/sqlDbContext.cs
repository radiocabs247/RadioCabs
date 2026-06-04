using Microsoft.EntityFrameworkCore;
using Radiocab.Models.SqlDb;

namespace Radiocab.Models.SqlDb
{
    public class SqlDbContext : DbContext
    {
        public SqlDbContext(DbContextOptions<SqlDbContext> options)
            : base(options)
        {
        }
        public DbSet<Users> Users { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Listing> Listings { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<Advertise> Advertisements { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Listing>()
                .Property(x => x.PaymentAmount)
                .HasPrecision(10, 2);
            modelBuilder.Entity<Listing>()
                .Property(x => x.Status)
                .HasDefaultValue("Pending");

            modelBuilder.Entity<Driver>()
                .Property(x => x.PaymentAmount)
                .HasPrecision(10, 2);
            modelBuilder.Entity<Driver>()
                .Property(x => x.Status)
                .HasDefaultValue("Pending");

            modelBuilder.Entity<Advertise>()
                .Property(x => x.PaymentAmount)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Payment>()
                .Property(x => x.Amount)
                .HasPrecision(10, 2);
        }
    }
}
