using CardService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardService.Infrastructure.Persistence
{
    public class CardServiceDbContext : DbContext
    {
        public DbSet<CreditCard> CreditCards => Set<CreditCard>();
        public DbSet<CardIssuer> CardIssuers => Set<CardIssuer>();
        public CardServiceDbContext(DbContextOptions<CardServiceDbContext> options)
            : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(CardServiceDbContext).Assembly);
        }
    }
}
