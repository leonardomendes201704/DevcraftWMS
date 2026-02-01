using FluentValidation;

namespace PerfectApiTemplate.Application.Features.Telemetry.Commands.ReportClientEvent;

public sealed class ReportClientEventCommandValidator : AbstractValidator<ReportClientEventCommand>
{
    public ReportClientEventCommandValidator()
    {
        RuleFor(x => x.EventType).NotEmpty().MaximumLength(128);
        RuleFor(x => x.Severity).NotEmpty().MaximumLength(32);
        RuleFor(x => x.ClientApp).NotEmpty().MaximumLength(128);
        RuleFor(x => x.ClientEnv).NotEmpty().MaximumLength(64);
        RuleFor(x => x.ClientUrl).NotEmpty().MaximumLength(2048);
        RuleFor(x => x.HttpMethod).NotEmpty().MaximumLength(16);
        RuleFor(x => x.CorrelationId).NotEmpty().MaximumLength(128);
        RuleFor(x => x.Message).NotEmpty().MaximumLength(4000);
        RuleFor(x => x.ClientRoute).MaximumLength(256);
        RuleFor(x => x.ApiMethod).MaximumLength(16);
        RuleFor(x => x.ApiPath).MaximumLength(1024);
        RuleFor(x => x.RequestId).MaximumLength(128);
        RuleFor(x => x.ApiRequestId).MaximumLength(128);
        RuleFor(x => x.UserId).MaximumLength(128);
        RuleFor(x => x.TenantId).MaximumLength(128);
        RuleFor(x => x.UserAgent).MaximumLength(512);
        RuleFor(x => x.ClientIp).MaximumLength(64);
        RuleFor(x => x.Tags).MaximumLength(512);
    }
}
