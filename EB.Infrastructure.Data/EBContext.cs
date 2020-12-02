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

            modelBuilder.Entity<User>()
                .HasOne(c => c.Customer)
                .WithOne(c => c.User).HasForeignKey<Customer>(c => c.ID).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderBeer>()
                .HasOne(ob => ob.Beer)
                .WithMany(b => b.OrderBeers);

            modelBuilder.Entity<OrderBeer>()
                .HasOne(ob => ob.Order)
                .WithMany(o => o.OrderBeers);

            modelBuilder.Entity<OrderBeer>()
                .HasKey(ob => new { ob.OrderID, ob.BeerID });

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.Orders);
        }

        public DbSet<Beer> Beers { get; set; }
        public DbSet<BeerType> Types { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Costumers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderBeer> OrderBeers { get; set; }
    }
}
