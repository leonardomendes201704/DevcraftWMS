using FluentValidation;

namespace DevcraftWMS.Application.Features.InboundOrders.Commands.ConvertAsnToInboundOrder;

public sealed class ConvertAsnToInboundOrderCommandValidator : AbstractValidator<ConvertAsnToInboundOrderCommand>
{
    public ConvertAsnToInboundOrderCommandValidator()
    {
        RuleFor(x => x.AsnId).NotEmpty();
    }
}
