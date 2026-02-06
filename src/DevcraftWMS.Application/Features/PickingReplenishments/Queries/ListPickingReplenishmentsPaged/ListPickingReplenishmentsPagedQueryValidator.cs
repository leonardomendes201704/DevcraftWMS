using FluentValidation;

namespace DevcraftWMS.Application.Features.PickingReplenishments.Queries.ListPickingReplenishmentsPaged;

public sealed class ListPickingReplenishmentsPagedQueryValidator : AbstractValidator<ListPickingReplenishmentsPagedQuery>
{
    public ListPickingReplenishmentsPagedQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.OrderBy).NotEmpty();
        RuleFor(x => x.OrderDir).NotEmpty();
    }
}
