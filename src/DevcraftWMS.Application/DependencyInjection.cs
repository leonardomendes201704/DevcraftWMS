using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using DevcraftWMS.Application.Common.Behaviors;
using DevcraftWMS.Application.Features.Customers;
using DevcraftWMS.Application.Features.Notifications;
using DevcraftWMS.Application.Abstractions.Notifications;
using DevcraftWMS.Application.Abstractions.Auth;
using DevcraftWMS.Application.Features.Auth;
using DevcraftWMS.Application.Features.Emails;
using DevcraftWMS.Application.Features.Warehouses;

namespace DevcraftWMS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IOutboxEnqueuer, OutboxEnqueuer>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IWarehouseService, WarehouseService>();

        return services;
    }
}


