using BillingService.Application.Abstractions;
using BillingService.Domain.Interfaces;
using BillingService.Infrastructure.Messaging.Consumers;
using BillingService.Infrastructure.Messaging.Publishers;
using BillingService.Infrastructure.Persistence;
using BillingService.Infrastructure.Persistence.Repositories;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillingService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<BillingServiceDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("BillingDb"),
                sql => sql.MigrationsAssembly(
                    typeof(BillingServiceDbContext).Assembly.FullName)));

            services.AddScoped<IBillRepository, BillRepository>();
            services.AddScoped<IPaymentScheduleRepository, PaymentScheduleRepository>();
            services.AddScoped<IRewardRepository, RewardRepository>();
            services.AddScoped<IEventPublisher, EventPublisher>();

            services.AddMassTransit(bus =>
            {
                bus.AddConsumer<PaymentCompletedConsumer>();
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
