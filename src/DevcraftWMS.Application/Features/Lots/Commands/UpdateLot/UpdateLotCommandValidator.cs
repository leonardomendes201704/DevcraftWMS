using FluentValidation;

namespace DevcraftWMS.Application.Features.Lots.Commands.UpdateLot;

public sealed class UpdateLotCommandValidator : AbstractValidator<UpdateLotCommand>
{
    public UpdateLotCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.ManufactureDate)
            .LessThanOrEqualTo(x => x.ExpirationDate)
            .When(x => x.ManufactureDate.HasValue && x.ExpirationDate.HasValue)
            .WithMessage("Expiration date cannot be earlier than manufacture date.");
    }
}
