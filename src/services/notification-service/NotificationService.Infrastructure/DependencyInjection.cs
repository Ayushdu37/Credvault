using NotificationService.Application.Abstractions;
using NotificationService.Domain.Interfaces;
using NotificationService.Infrastructure.Messaging.Consumers;
using NotificationService.Infrastructure.Persistence;
using NotificationService.Infrastructure.Persistence.Repositories;
using NotificationService.Infrastructure.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<NotificationServiceDbContext>(options =>
            options.UseSqlServer(
                config.GetConnectionString("NotificationDb"),
                sql => sql.MigrationsAssembly(
                    typeof(NotificationServiceDbContext).Assembly.FullName)));

            services.AddScoped<INotificationRepository,
            NotificationRepository>();
            services.AddScoped<INotificationPreferenceRepository,
                NotificationPreferenceRepository>();
            services.AddScoped<IEmailService, EmailService>();

            services.AddMassTransit(bus =>
            {
                bus.AddConsumer<PaymentCompletedConsumer>();
                bus.AddConsumer<PaymentFailedConsumer>();
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
