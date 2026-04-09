using NotificationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Infrastructure.Persistence
{
    public class NotificationServiceDbContext : DbContext
    {
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<NotificationPreference> NotificationPreferences
            => Set<NotificationPreference>();

        public NotificationServiceDbContext(
            DbContextOptions<NotificationServiceDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(NotificationServiceDbContext).Assembly);
        }
    }
}
