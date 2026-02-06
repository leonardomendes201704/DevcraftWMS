using FluentValidation;

namespace DevcraftWMS.Application.Features.DockSchedules.Queries.ListDockSchedulesPaged;

public sealed class ListDockSchedulesPagedQueryValidator : AbstractValidator<ListDockSchedulesPagedQuery>
{
    public ListDockSchedulesPagedQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 200);

        RuleFor(x => x.OrderBy)
            .NotEmpty();

        RuleFor(x => x.OrderDir)
            .NotEmpty();
    }
}
