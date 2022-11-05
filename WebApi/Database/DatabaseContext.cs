using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using WebApi.Database.Models;

namespace WebApi.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bill>().ToTable("Bills");
            modelBuilder.Entity<Recipient>().ToTable("Recipients");
            modelBuilder.Entity<User>().ToTable("Users");

            modelBuilder.Entity<User>()
           .HasMany(p => p.Bills)
           .WithOne(b => b.User);

            modelBuilder.Entity<User>()
           .HasMany(p => p.Recipients)
           .WithOne(b => b.User);
        }

        public DbSet<Bill> Bills { get; set; }
        public DbSet<Recipient> Recipients { get; set; }
        public DbSet<User> Users { get; set; }
    }
}