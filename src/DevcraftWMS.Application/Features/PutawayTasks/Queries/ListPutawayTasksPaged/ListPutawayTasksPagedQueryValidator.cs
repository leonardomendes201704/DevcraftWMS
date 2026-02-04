using FluentValidation;

namespace DevcraftWMS.Application.Features.PutawayTasks.Queries.ListPutawayTasksPaged;

public sealed class ListPutawayTasksPagedQueryValidator : AbstractValidator<ListPutawayTasksPagedQuery>
{
    public ListPutawayTasksPagedQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.OrderBy).NotEmpty();
        RuleFor(x => x.OrderDir).NotEmpty();
    }
}
