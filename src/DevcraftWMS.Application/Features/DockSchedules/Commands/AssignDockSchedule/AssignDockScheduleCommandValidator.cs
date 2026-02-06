using FluentValidation;

namespace DevcraftWMS.Application.Features.DockSchedules.Commands.AssignDockSchedule;

public sealed class AssignDockScheduleCommandValidator : AbstractValidator<AssignDockScheduleCommand>
{
    public AssignDockScheduleCommandValidator()
    {
        RuleFor(x => x.DockScheduleId)
            .NotEmpty();

        RuleFor(x => x.Assignment)
            .NotNull();
    }
}
