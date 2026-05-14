using CardService.Application.Abstractions;
using CardService.Domain.Interfaces;
using CardService.Infrastructure.Messaging.Publishers;
using CardService.Infrastructure.Persistence;
using CardService.Infrastructure.Persistence.Repositories;
using CardService.Infrastructure.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, IConfiguration config)
        {
            // Database
            services.AddDbContext<CardServiceDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("CardDb"),
                    sql => sql.MigrationsAssembly(
                        typeof(CardServiceDbContext).Assembly.FullName)));

            // Repositories
            services.AddScoped<ICreditCardRepository, CreditCardRepository>();
            services.AddScoped<ICardIssuerRepository, CardIssuerRepository>();

            // Services
            services.AddScoped<ICardHasher, CardHasher>();
            services.AddScoped<IEventPublisher, EventPublisher>();

            // MassTransit (RabbitMQ)
            services.AddMassTransit(bus =>
            {
                bus.AddConsumer<CardService.Infrastructure.Messaging.Consumers.PaymentCompletedConsumer>();

                bus.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(config["RabbitMQ:Host"] ?? "localhost", "/", h =>
                    {
                        h.Username(config["RabbitMQ:Username"] ?? "guest");
                        h.Password(config["RabbitMQ:Password"] ?? "guest");
                    });
                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}
