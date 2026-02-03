using FluentValidation;

namespace DevcraftWMS.Application.Features.UnitLoads.Queries.ListUnitLoadsPaged;

public sealed class ListUnitLoadsPagedQueryValidator : AbstractValidator<ListUnitLoadsPagedQuery>
{
    public ListUnitLoadsPagedQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).GreaterThan(0).LessThanOrEqualTo(100);
        RuleFor(x => x.OrderBy).NotEmpty().MaximumLength(100);
        RuleFor(x => x.OrderDir).NotEmpty().MaximumLength(4);
        RuleFor(x => x.Sscc).MaximumLength(50);
    }
}
