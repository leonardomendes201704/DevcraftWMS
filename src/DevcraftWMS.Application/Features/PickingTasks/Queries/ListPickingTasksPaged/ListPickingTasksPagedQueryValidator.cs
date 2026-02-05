using FluentValidation;

namespace DevcraftWMS.Application.Features.PickingTasks.Queries.ListPickingTasksPaged;

public sealed class ListPickingTasksPagedQueryValidator : AbstractValidator<ListPickingTasksPagedQuery>
{
    public ListPickingTasksPagedQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.OrderBy).NotEmpty();
        RuleFor(x => x.OrderDir).NotEmpty();
    }
}
