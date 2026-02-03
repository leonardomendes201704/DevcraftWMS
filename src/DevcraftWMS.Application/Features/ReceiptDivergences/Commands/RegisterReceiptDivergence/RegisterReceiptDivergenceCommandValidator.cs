using FluentValidation;

namespace DevcraftWMS.Application.Features.ReceiptDivergences.Commands.RegisterReceiptDivergence;

public sealed class RegisterReceiptDivergenceCommandValidator : AbstractValidator<RegisterReceiptDivergenceCommand>
{
    public RegisterReceiptDivergenceCommandValidator()
    {
        RuleFor(x => x.ReceiptId).NotEmpty();
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}
