using FluentValidation;

namespace DevcraftWMS.Application.Features.Returns.Commands.CompleteReturnOrder;

public sealed class CompleteReturnOrderCommandValidator : AbstractValidator<CompleteReturnOrderCommand>
{
    public CompleteReturnOrderCommandValidator()
    {
        RuleFor(x => x.ReturnOrderId)
            .NotEmpty();

        RuleFor(x => x.Items)
            .NotNull()
            .Must(items => items.Count > 0)
            .WithMessage("At least one item is required.");
    }
}
