using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EB.Infrastructure.Data
{
    public class EBContext : DbContext
    {
        public EBContext(DbContextOptions<EBContext> option) : base(option) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }

        public DbSet<Type> Types { get; set; }
    }
}
