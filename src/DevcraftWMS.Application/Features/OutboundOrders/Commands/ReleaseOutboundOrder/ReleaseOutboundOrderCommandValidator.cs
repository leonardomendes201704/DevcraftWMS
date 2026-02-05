using DevcraftWMS.Domain.Enums;
using FluentValidation;

namespace DevcraftWMS.Application.Features.OutboundOrders.Commands.ReleaseOutboundOrder;

public sealed class ReleaseOutboundOrderCommandValidator : AbstractValidator<ReleaseOutboundOrderCommand>
{
    public ReleaseOutboundOrderCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Priority).IsInEnum();
        RuleFor(x => x.PickingMethod).IsInEnum();

        RuleFor(x => x)
            .Must(x => !(x.ShippingWindowStartUtc.HasValue ^ x.ShippingWindowEndUtc.HasValue))
            .WithMessage("Shipping window requires both start and end values.");

        RuleFor(x => x)
            .Must(x => !x.ShippingWindowStartUtc.HasValue || !x.ShippingWindowEndUtc.HasValue ||
                       x.ShippingWindowEndUtc.Value >= x.ShippingWindowStartUtc.Value)
            .WithMessage("Shipping window end must be after start.");
    }
}

