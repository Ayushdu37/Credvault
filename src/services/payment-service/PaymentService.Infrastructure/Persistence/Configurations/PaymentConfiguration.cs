using PaymentService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Infrastructure.Persistence.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Amount)
                .HasColumnType("decimal(18,2)");
            builder.Property(p => p.PaymentMethod)
                .IsRequired().HasMaxLength(30);
            builder.Property(p => p.TransactionReference)
                .HasMaxLength(100);
            builder.Property(p => p.Status)
                .IsRequired().HasMaxLength(20);
            builder.Property(p => p.FailureReason)
                .HasMaxLength(500);

            builder.HasIndex(p => p.UserId);
            builder.HasIndex(p => p.BillId);
            builder.HasIndex(p => p.Status);
        }
    }
}
