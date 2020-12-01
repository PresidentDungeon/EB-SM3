using EB.Core.Entities;
using EB.Core.Entities.Security;
using Microsoft.EntityFrameworkCore;

namespace EB.Infrastructure.Data
{
    public class EBContext : DbContext
    {
        public EBContext(DbContextOptions<EBContext> option) : base(option) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Beer>()
                .HasOne(b => b.Type)
                .WithMany(bt => bt.Beers).OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Beer>()
                .HasOne(b => b.Brand)
                .WithMany(bb => bb.Beers).OnDelete(DeleteBehavior.SetNull);

            //modelBuilder.Entity<User>()
            //    .HasOne(u => u.Customer);


            modelBuilder.Entity<User>()
                .HasOne(c => c.Customer)
                .WithOne(c => c.User).HasForeignKey<Customer>(c => c.ID).OnDelete(DeleteBehavior.Cascade);
        }

        public DbSet<Beer> Beers { get; set; }
        public DbSet<BeerType> Types { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Costumers { get; set; }
    }
}
