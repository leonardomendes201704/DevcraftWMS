using FluentValidation;

namespace DevcraftWMS.Application.Features.InboundOrders.Commands.CompleteInboundOrder;

public sealed class CompleteInboundOrderCommandValidator : AbstractValidator<CompleteInboundOrderCommand>
{
    public CompleteInboundOrderCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("InboundOrderId is required.");
    }
}
