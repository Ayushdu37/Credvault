using BillingService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillingService.Infrastructure.Persistence.Configurations
{
    public class BillConfiguration : IEntityTypeConfiguration<Bill>
    {
        public void Configure(EntityTypeBuilder<Bill> builder)
        {
            builder.ToTable("Bills");
            builder.HasKey(b => b.Id);

            builder.Property(b => b.TotalAmount).HasColumnType("decimal(18,2)");
            builder.Property(b => b.MinimumDue).HasColumnType("decimal(18,2)");
            builder.Property(b => b.AmountPaid)
                .HasColumnType("decimal(18,2)").HasDefaultValue(0);

            builder.Property(b => b.BillingMonth)
                .IsRequired().HasMaxLength(7);
            builder.Property(b => b.Status)
                .IsRequired().HasMaxLength(20);

            builder.Property(b => b.IsDeleted).HasDefaultValue(false);

            builder.HasIndex(b => b.UserId);
            builder.HasIndex(b => b.CardId);
            builder.HasIndex(b => new { b.CardId, b.BillingMonth }).IsUnique();

            builder.HasQueryFilter(b => !b.IsDeleted);
        }
    }
}
