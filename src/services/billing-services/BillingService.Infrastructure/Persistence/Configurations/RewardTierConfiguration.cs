using BillingService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillingService.Infrastructure.Persistence.Configurations
{
    public class RewardTierConfiguration : IEntityTypeConfiguration<RewardTier>
    {
        public static readonly Guid SilverId =
            Guid.Parse("b1b2c3d4-0001-0001-0001-000000000001");
        public static readonly Guid GoldId =
            Guid.Parse("b1b2c3d4-0001-0001-0001-000000000002");
        public static readonly Guid PlatinumId =
            Guid.Parse("b1b2c3d4-0001-0001-0001-000000000003");

        public void Configure(EntityTypeBuilder<RewardTier> builder)
        {
            builder.ToTable("RewardTiers");
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name)
                .IsRequired().HasMaxLength(20);
            builder.HasIndex(t => t.Name).IsUnique();

            builder.Property(t => t.CashbackPercent)
                .HasColumnType("decimal(4,2)");

            var seedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            builder.HasData(
                new
                {
                    Id = SilverId,
                    Name = "Silver",
                    MinPoints = 0,
                    CashbackPercent = 0.50m,
                    CreatedAt = seedDate
                },
                new
                {
                    Id = GoldId,
                    Name = "Gold",
                    MinPoints = 5000,
                    CashbackPercent = 1.00m,
                    CreatedAt = seedDate
                },
                new
                {
                    Id = PlatinumId,
                    Name = "Platinum",
                    MinPoints = 15000,
                    CashbackPercent = 2.00m,
                    CreatedAt = seedDate
                }
            );
        }
    }
}
