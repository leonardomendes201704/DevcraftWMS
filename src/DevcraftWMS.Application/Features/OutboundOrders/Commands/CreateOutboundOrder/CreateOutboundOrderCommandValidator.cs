using FluentValidation;

namespace DevcraftWMS.Application.Features.OutboundOrders.Commands.CreateOutboundOrder;

public sealed class CreateOutboundOrderCommandValidator : AbstractValidator<CreateOutboundOrderCommand>
{
    public CreateOutboundOrderCommandValidator()
    {
        RuleFor(x => x.WarehouseId)
            .NotEmpty()
            .WithMessage("WarehouseId is required.");

        RuleFor(x => x.OrderNumber)
            .NotEmpty()
            .WithMessage("OrderNumber is required.")
            .MaximumLength(32)
            .WithMessage("OrderNumber must be 32 characters or fewer.");

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("At least one item is required.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId)
                .NotEmpty()
                .WithMessage("ProductId is required.");
            item.RuleFor(i => i.UomId)
                .NotEmpty()
                .WithMessage("UomId is required.");
            item.RuleFor(i => i.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than zero.");
        });
    }
}
