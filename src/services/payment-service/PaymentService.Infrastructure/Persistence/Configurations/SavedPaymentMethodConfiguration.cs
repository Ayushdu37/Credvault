using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Infrastructure.Persistence.Configurations
{
    public class SavedPaymentMethodConfiguration
    : IEntityTypeConfiguration<SavedPaymentMethod>
    {
        public void Configure(EntityTypeBuilder<SavedPaymentMethod> builder)
        {
            builder.ToTable("SavedPaymentMethods");
            builder.HasKey(m => m.Id);

            builder.Property(m => m.MethodType)
                .IsRequired().HasMaxLength(30);
            builder.Property(m => m.DisplayName)
                .IsRequired().HasMaxLength(100);
            builder.Property(m => m.Details)
                .IsRequired().HasMaxLength(200);

            builder.HasIndex(m => m.UserId);
        }
    }
}
