using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotificationService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Infrastructure.Persistence.Configurations
{
    public class NotificationPreferenceConfiguration
    : IEntityTypeConfiguration<NotificationPreference>
    {
        public void Configure(EntityTypeBuilder<NotificationPreference> builder)
        {
            builder.ToTable("NotificationPreferences");
            builder.HasKey(p => p.Id);

            builder.HasIndex(p => p.UserId).IsUnique();
        }
    }
}
