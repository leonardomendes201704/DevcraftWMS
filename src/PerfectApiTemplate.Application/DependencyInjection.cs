using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PerfectApiTemplate.Application.Common.Behaviors;
using PerfectApiTemplate.Application.Features.Customers;
using PerfectApiTemplate.Application.Features.Notifications;
using PerfectApiTemplate.Application.Abstractions.Notifications;
using PerfectApiTemplate.Application.Abstractions.Auth;
using PerfectApiTemplate.Application.Features.Auth;
using PerfectApiTemplate.Application.Features.Emails;

namespace PerfectApiTemplate.Application;

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

        return services;
    }
}

