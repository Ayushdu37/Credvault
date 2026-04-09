using IdentityService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityService.Infrastructure.Persistence.Configurations
{
    public class OTPCodeConfiguration : IEntityTypeConfiguration<OTPCode>
    {
        public void Configure(EntityTypeBuilder<OTPCode> builder)
        {
            builder.ToTable("OTPCodes");

            builder.HasKey(otp => otp.Id);

            builder.Property(otp => otp.Code)
                .IsRequired()
                .HasMaxLength(6);

            builder.Property(otp => otp.Purpose)
                .HasConversion<string>()
                .HasMaxLength(30);

            // Composite index — fast lookup of latest OTP by user + purpose
            builder.HasIndex(otp => new { otp.UserId, otp.Purpose });
        }
    }
}
