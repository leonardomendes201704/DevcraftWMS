using FluentValidation;

namespace DevcraftWMS.Application.Features.UnitLoads.Commands.RelabelUnitLoadLabel;

public sealed class RelabelUnitLoadLabelCommandValidator : AbstractValidator<RelabelUnitLoadLabelCommand>
{
    public RelabelUnitLoadLabelCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}
