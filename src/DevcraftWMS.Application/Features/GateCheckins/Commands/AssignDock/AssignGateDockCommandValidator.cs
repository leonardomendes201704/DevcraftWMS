using FluentValidation;

namespace DevcraftWMS.Application.Features.GateCheckins.Commands.AssignDock;

public sealed class AssignGateDockCommandValidator : AbstractValidator<AssignGateDockCommand>
{
    public AssignGateDockCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.DockCode)
            .NotEmpty()
            .MaximumLength(20);
    }
}
