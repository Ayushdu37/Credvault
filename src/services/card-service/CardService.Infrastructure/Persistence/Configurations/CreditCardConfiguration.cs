using CardService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardService.Infrastructure.Persistence.Configurations
{
    public class CreditCardConfiguration : IEntityTypeConfiguration<CreditCard>
    {
        public void Configure(EntityTypeBuilder<CreditCard> builder)
        {
            builder.ToTable("CreditCards");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.MaskedNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(c => c.CardNumberHash)
                .IsRequired()
                .HasMaxLength(512);

            builder.Property(c => c.CardHolderName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.CreditLimit)
            .HasColumnType("decimal(18,2)");

            builder.Property(c => c.OutstandingBalance)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);

            builder.Property(c => c.IsDefault)
                .HasDefaultValue(false);

            builder.Property(c => c.IsVerified)
                .HasDefaultValue(false);

            builder.Property(c => c.IsDeleted)
                .HasDefaultValue(false);

            // Indexes
            builder.HasIndex(c => c.UserId);
            builder.HasIndex(c => new { c.UserId, c.CardNumberHash }).IsUnique();

            // Relationship: CreditCard -> CardIssuer
            builder.HasOne(c => c.Issuer)
                .WithMany(i => i.CreditCards)
                .HasForeignKey(c => c.IssuerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Global query filter: exclude soft-deleted cards by default
            builder.HasQueryFilter(c => !c.IsDeleted);
        }
    }
}
