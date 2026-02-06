using FluentValidation;

namespace DevcraftWMS.Application.Features.OutboundChecks.Commands.StartOutboundCheck;

public sealed class StartOutboundCheckCommandValidator : AbstractValidator<StartOutboundCheckCommand>
{
    public StartOutboundCheckCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
