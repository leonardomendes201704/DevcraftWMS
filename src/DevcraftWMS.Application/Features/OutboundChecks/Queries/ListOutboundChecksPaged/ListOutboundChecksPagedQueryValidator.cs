using FluentValidation;

namespace DevcraftWMS.Application.Features.OutboundChecks.Queries.ListOutboundChecksPaged;

public sealed class ListOutboundChecksPagedQueryValidator : AbstractValidator<ListOutboundChecksPagedQuery>
{
    public ListOutboundChecksPagedQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.OrderBy).NotEmpty();
        RuleFor(x => x.OrderDir).NotEmpty();
    }
}
