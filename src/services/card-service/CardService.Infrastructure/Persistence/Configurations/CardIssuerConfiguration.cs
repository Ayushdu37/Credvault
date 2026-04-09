using CardService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardService.Infrastructure.Persistence.Configurations
{
    public class CardIssuerConfiguration : IEntityTypeConfiguration<CardIssuer>
    {
        // Fixed GUIDs for seed data — never change these!
        public static readonly Guid VisaId = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000001");
        public static readonly Guid MastercardId = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000002");
        public static readonly Guid AmexId = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000003");
        public static readonly Guid RuPayId = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000004");

        public void Configure(EntityTypeBuilder<CardIssuer> builder)
        {
            builder.ToTable("CardIssuers");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(i => i.Name).IsUnique();

            builder.Property(i => i.BinPrefixes)
                .IsRequired()
                .HasMaxLength(200);

            // Seed data — use anonymous objects with hardcoded dates (DateTime.UtcNow breaks EF migrations)
            var seedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            builder.HasData(
                new { Id = VisaId, Name = "Visa", CardLength = 16, BinPrefixes = "4", CreatedAt = seedDate },
                new { Id = MastercardId, Name = "Mastercard", CardLength = 16, BinPrefixes = "51,52,53,54,55,2221-2720", CreatedAt = seedDate },
                new { Id = AmexId, Name = "Amex", CardLength = 15, BinPrefixes = "34,37", CreatedAt = seedDate },
                new { Id = RuPayId, Name = "RuPay", CardLength = 16, BinPrefixes = "60,65,81,82,508", CreatedAt = seedDate }
            );
        }
    }
}
