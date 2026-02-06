using FluentValidation;

namespace DevcraftWMS.Application.Features.DockSchedules.Commands.CreateDockSchedule;

public sealed class CreateDockScheduleCommandValidator : AbstractValidator<CreateDockScheduleCommand>
{
    public CreateDockScheduleCommandValidator()
    {
        RuleFor(x => x.WarehouseId)
            .NotEmpty();

        RuleFor(x => x.DockCode)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.SlotEndUtc)
            .GreaterThan(x => x.SlotStartUtc);
    }
}
