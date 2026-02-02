using FluentValidation;

namespace DevcraftWMS.Application.Features.InventoryBalances.Commands.UpdateInventoryBalance;

public sealed class UpdateInventoryBalanceCommandValidator : AbstractValidator<UpdateInventoryBalanceCommand>
{
    public UpdateInventoryBalanceCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.LocationId).NotEmpty();
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.QuantityOnHand).GreaterThanOrEqualTo(0);
        RuleFor(x => x.QuantityReserved).GreaterThanOrEqualTo(0);
        RuleFor(x => x).Must(x => x.QuantityReserved <= x.QuantityOnHand)
            .WithMessage("Reserved quantity cannot exceed on-hand quantity.");
    }
}
