using FluentValidation;

namespace DevcraftWMS.Application.Features.Uoms.Commands.CreateUom;

public sealed class CreateUomCommandValidator : AbstractValidator<CreateUomCommand>
{
    public CreateUomCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(16);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}
