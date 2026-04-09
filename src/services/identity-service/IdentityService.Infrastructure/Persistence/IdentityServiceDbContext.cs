using IdentityService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityService.Infrastructure.Persistence
{
    public class IdentityServiceDbContext : DbContext
    {
        // Each DbSet<T> = a table in the database
        public DbSet<User> Users => Set<User>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<OTPCode> OTPCodes => Set<OTPCode>();

        // Constructor — EF Core passes the connection string through options
        public IdentityServiceDbContext(DbContextOptions<IdentityServiceDbContext> options)
            : base(options)
        {
        }

        // This is where we tell EF Core HOW to map our entities to tables
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all entity configurations from this assembly
            // (it auto-discovers UserConfiguration, RefreshTokenConfiguration, etc.)
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityServiceDbContext).Assembly);
        }
    }
}
