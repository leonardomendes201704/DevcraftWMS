using FluentValidation;

namespace DevcraftWMS.Application.Features.QualityInspections.Commands.AddQualityInspectionEvidence;

public sealed class AddQualityInspectionEvidenceCommandValidator : AbstractValidator<AddQualityInspectionEvidenceCommand>
{
    public AddQualityInspectionEvidenceCommandValidator()
    {
        RuleFor(x => x.InspectionId).NotEmpty();
        RuleFor(x => x.FileName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ContentType).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Content).NotNull().Must(c => c.Length > 0).WithMessage("Evidence content is required.");
    }
}
