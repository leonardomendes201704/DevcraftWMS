using FluentValidation;

namespace DevcraftWMS.Application.Features.OutboundOrders.Commands.CancelOutboundOrder;

public sealed class CancelOutboundOrderCommandValidator : AbstractValidator<CancelOutboundOrderCommand>
{
    public CancelOutboundOrderCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
    }
}
