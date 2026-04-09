using IdentityService.Application.Abstraction;
using IdentityService.Domain.Interfaces;
using IdentityService.Infrastructure.Messaging.Publishers;
using IdentityService.Infrastructure.Persistence;
using IdentityService.Infrastructure.Persistence.Repositories;
using IdentityService.Infrastructure.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, IConfiguration configuration)
        {
            // --- Database ---
            services.AddDbContext<IdentityServiceDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("IdentityDb"),
                    sqlOptions => sqlOptions.MigrationsAssembly(
                        typeof(IdentityServiceDbContext).Assembly.FullName)));

            // --- Repositories ---
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IOTPCodeRepository, OTPCodeRepository>();

            // --- Services ---
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IEventPublisher, EventPublisher>();

            // --- MassTransit (RabbitMQ) ---
            services.AddMassTransit(busConfig =>
            {
                busConfig.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(configuration["RabbitMQ:Host"] ?? "localhost", "/", h =>
                    {
                        h.Username(configuration["RabbitMQ:Username"] ?? "guest");
                        h.Password(configuration["RabbitMQ:Password"] ?? "guest");
                    });
                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}
