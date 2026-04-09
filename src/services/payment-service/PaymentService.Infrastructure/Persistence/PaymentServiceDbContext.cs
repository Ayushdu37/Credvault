using PaymentService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Infrastructure.Persistence
{
    public class PaymentServiceDbContext : DbContext
    {
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<SavedPaymentMethod> SavedPaymentMethods
            => Set<SavedPaymentMethod>();

        public PaymentServiceDbContext(
        DbContextOptions<PaymentServiceDbContext> options)
        : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(PaymentServiceDbContext).Assembly);
        }
    }
}
