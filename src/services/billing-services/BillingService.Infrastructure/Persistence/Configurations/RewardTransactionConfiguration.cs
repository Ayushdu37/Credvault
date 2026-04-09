using BillingService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillingService.Infrastructure.Persistence.Configurations
{
    public class RewardTransactionConfiguration : IEntityTypeConfiguration<RewardTransaction>
    {
        public void Configure(EntityTypeBuilder<RewardTransaction> builder)
        {
            builder.ToTable("RewardTransactions");
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Type)
                .IsRequired().HasMaxLength(20);
            builder.Property(t => t.Description).HasMaxLength(200);

            builder.HasOne(t => t.RewardAccount)
                .WithMany(a => a.Transactions)
                .HasForeignKey(t => t.RewardAccountId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
