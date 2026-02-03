using FluentValidation;

namespace DevcraftWMS.Application.Features.UnitLoads.Commands.CreateUnitLoad;

public sealed class CreateUnitLoadCommandValidator : AbstractValidator<CreateUnitLoadCommand>
{
    public CreateUnitLoadCommandValidator()
    {
        RuleFor(x => x.ReceiptId).NotEmpty();
        RuleFor(x => x.SsccExternal).MaximumLength(50);
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}
