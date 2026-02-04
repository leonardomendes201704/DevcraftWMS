using FluentValidation;

namespace DevcraftWMS.Application.Features.Receipts.Commands.AddReceiptItem;

public sealed class AddReceiptItemCommandValidator : AbstractValidator<AddReceiptItemCommand>
{
    public AddReceiptItemCommandValidator()
    {
        RuleFor(x => x.ReceiptId).NotEmpty();
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.LotCode)
            .MaximumLength(50)
            .When(x => !string.IsNullOrWhiteSpace(x.LotCode));
        RuleFor(x => x.LocationId).NotEmpty();
        RuleFor(x => x.UomId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.UnitCost).GreaterThanOrEqualTo(0).When(x => x.UnitCost.HasValue);
    }
}
