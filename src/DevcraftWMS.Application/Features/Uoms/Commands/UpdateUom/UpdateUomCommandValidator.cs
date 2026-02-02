using FluentValidation;

namespace DevcraftWMS.Application.Features.Uoms.Commands.UpdateUom;

public sealed class UpdateUomCommandValidator : AbstractValidator<UpdateUomCommand>
{
    public UpdateUomCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Code).NotEmpty().MaximumLength(16);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}
