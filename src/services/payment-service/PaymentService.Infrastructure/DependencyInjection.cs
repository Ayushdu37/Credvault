using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentService.Application.Abstractions;
using PaymentService.Domain.Interfaces;
using PaymentService.Infrastructure.Messaging.Publishers;
using PaymentService.Infrastructure.Persistence;
using PaymentService.Infrastructure.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<PaymentServiceDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("PaymentDb"),
                sql => sql.MigrationsAssembly(
                    typeof(PaymentServiceDbContext).Assembly.FullName)));

            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<ISavedPaymentMethodRepository,
                SavedPaymentMethodRepository>();
            services.AddScoped<IEventPublisher, EventPublisher>();

            services.AddMassTransit(bus =>
            {
                // No consumers in Payment Service
                // (it only publishes, doesn't consume)
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
