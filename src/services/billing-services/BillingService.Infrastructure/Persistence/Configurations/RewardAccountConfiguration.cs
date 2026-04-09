using BillingService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillingService.Infrastructure.Persistence.Configurations
{
    public class RewardAccountConfiguration : IEntityTypeConfiguration<RewardAccount>
    {
        public void Configure(EntityTypeBuilder<RewardAccount> builder)
        {
            builder.ToTable("RewardAccounts");
            builder.HasKey(a => a.Id);

            builder.HasIndex(a => a.UserId).IsUnique();

            builder.Property(a => a.AvailablePoints).HasDefaultValue(0);
            builder.Property(a => a.TotalEarned).HasDefaultValue(0);

            builder.HasOne(a => a.Tier)
                .WithMany(t => t.RewardAccounts)
                .HasForeignKey(a => a.TierId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
