using FluentValidation;

namespace DevcraftWMS.Application.Features.InboundOrders.Commands.CancelInboundOrder;

public sealed class CancelInboundOrderCommandValidator : AbstractValidator<CancelInboundOrderCommand>
{
    public CancelInboundOrderCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
    }
}
