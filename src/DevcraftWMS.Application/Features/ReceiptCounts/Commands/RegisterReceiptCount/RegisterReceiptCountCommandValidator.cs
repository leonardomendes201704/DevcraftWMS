using FluentValidation;

namespace DevcraftWMS.Application.Features.ReceiptCounts.Commands.RegisterReceiptCount;

public sealed class RegisterReceiptCountCommandValidator : AbstractValidator<RegisterReceiptCountCommand>
{
    public RegisterReceiptCountCommandValidator()
    {
        RuleFor(x => x.ReceiptId).NotEmpty();
        RuleFor(x => x.InboundOrderItemId).NotEmpty();
        RuleFor(x => x.CountedQuantity).GreaterThan(0);
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}
