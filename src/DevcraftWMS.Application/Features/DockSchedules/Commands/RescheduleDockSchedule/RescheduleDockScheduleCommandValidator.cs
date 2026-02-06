using FluentValidation;

namespace DevcraftWMS.Application.Features.DockSchedules.Commands.RescheduleDockSchedule;

public sealed class RescheduleDockScheduleCommandValidator : AbstractValidator<RescheduleDockScheduleCommand>
{
    public RescheduleDockScheduleCommandValidator()
    {
        RuleFor(x => x.DockScheduleId)
            .NotEmpty();

        RuleFor(x => x.SlotEndUtc)
            .GreaterThan(x => x.SlotStartUtc);

        RuleFor(x => x.Reason)
            .NotEmpty()
            .MaximumLength(300);
    }
}
