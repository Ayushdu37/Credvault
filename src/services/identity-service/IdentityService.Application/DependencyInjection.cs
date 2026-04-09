using FluentValidation;
using IdentityService.Application.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace IdentityService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            // Register all MediatR handlers in this assembly
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

            // Register all FluentValidation validators in this assembly
            services.AddValidatorsFromAssembly(assembly);

            // Register the validation pipeline behavior
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            return services;
        }
    }
}
