using BillingService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillingService.Infrastructure.Persistence.Configurations
{
    public class PaymentScheduleConfiguration : IEntityTypeConfiguration<PaymentSchedule>
    {
        public void Configure(EntityTypeBuilder<PaymentSchedule> builder)
        {
            builder.ToTable("PaymentSchedules");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Amount).HasColumnType("decimal(18,2)");
            builder.Property(p => p.Status)
                .IsRequired().HasMaxLength(20);

            builder.HasIndex(p => p.UserId);

            builder.HasOne(p => p.Bill)
                .WithMany(b => b.PaymentSchedules)
                .HasForeignKey(p => p.BillId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
