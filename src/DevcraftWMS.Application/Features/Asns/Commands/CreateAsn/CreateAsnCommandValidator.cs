using FluentValidation;

namespace DevcraftWMS.Application.Features.Asns.Commands.CreateAsn;

public sealed class CreateAsnCommandValidator : AbstractValidator<CreateAsnCommand>
{
    public CreateAsnCommandValidator()
    {
        RuleFor(x => x.WarehouseId)
            .NotEmpty()
            .WithMessage("WarehouseId is required.");

        RuleFor(x => x.AsnNumber)
            .NotEmpty()
            .WithMessage("AsnNumber is required.");
    }
}
