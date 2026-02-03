using FluentValidation;

namespace DevcraftWMS.Application.Features.Receipts.Commands.StartReceiptFromInboundOrder;

public sealed class StartReceiptFromInboundOrderCommandValidator : AbstractValidator<StartReceiptFromInboundOrderCommand>
{
    public StartReceiptFromInboundOrderCommandValidator()
    {
        RuleFor(x => x.InboundOrderId).NotEmpty();
    }
}
