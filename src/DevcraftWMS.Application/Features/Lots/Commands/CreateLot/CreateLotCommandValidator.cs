using FluentValidation;

namespace DevcraftWMS.Application.Features.Lots.Commands.CreateLot;

public sealed class CreateLotCommandValidator : AbstractValidator<CreateLotCommand>
{
    public CreateLotCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.ManufactureDate)
            .LessThanOrEqualTo(x => x.ExpirationDate)
            .When(x => x.ManufactureDate.HasValue && x.ExpirationDate.HasValue)
            .WithMessage("Expiration date cannot be earlier than manufacture date.");
    }
}
