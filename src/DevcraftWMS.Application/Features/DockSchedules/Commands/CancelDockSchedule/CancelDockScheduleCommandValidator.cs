using FluentValidation;

namespace DevcraftWMS.Application.Features.DockSchedules.Commands.CancelDockSchedule;

public sealed class CancelDockScheduleCommandValidator : AbstractValidator<CancelDockScheduleCommand>
{
    public CancelDockScheduleCommandValidator()
    {
        RuleFor(x => x.DockScheduleId)
            .NotEmpty();

        RuleFor(x => x.Reason)
            .NotEmpty()
            .MaximumLength(300);
    }
}
