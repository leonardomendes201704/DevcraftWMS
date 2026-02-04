using FluentValidation;

namespace DevcraftWMS.Application.Features.QualityInspections.Commands.RejectQualityInspection;

public sealed class RejectQualityInspectionCommandValidator : AbstractValidator<RejectQualityInspectionCommand>
{
    public RejectQualityInspectionCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Notes).MaximumLength(1000);
    }
}
