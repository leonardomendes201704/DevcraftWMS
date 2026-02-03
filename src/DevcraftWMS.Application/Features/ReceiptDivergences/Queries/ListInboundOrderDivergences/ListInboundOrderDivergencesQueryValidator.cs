using FluentValidation;

namespace DevcraftWMS.Application.Features.ReceiptDivergences.Queries.ListInboundOrderDivergences;

public sealed class ListInboundOrderDivergencesQueryValidator : AbstractValidator<ListInboundOrderDivergencesQuery>
{
    public ListInboundOrderDivergencesQueryValidator()
    {
        RuleFor(x => x.InboundOrderId).NotEmpty();
    }
}
