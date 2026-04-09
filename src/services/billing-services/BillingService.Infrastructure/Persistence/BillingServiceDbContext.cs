using BillingService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillingService.Infrastructure.Persistence
{
    public class BillingServiceDbContext : DbContext
    {
        public DbSet<Bill> Bills => Set<Bill>();
        public DbSet<PaymentSchedule> PaymentSchedules => Set<PaymentSchedule>();
        public DbSet<RewardTier> RewardTiers => Set<RewardTier>();
        public DbSet<RewardAccount> RewardAccounts => Set<RewardAccount>();
        public DbSet<RewardTransaction> RewardTransactions => Set<RewardTransaction>();

        public BillingServiceDbContext(
        DbContextOptions<BillingServiceDbContext> options)
        : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(BillingServiceDbContext).Assembly);
        }
    }
}
