using FluentValidation;

namespace DevcraftWMS.Application.Features.QualityInspections.Commands.ApproveQualityInspection;

public sealed class ApproveQualityInspectionCommandValidator : AbstractValidator<ApproveQualityInspectionCommand>
{
    public ApproveQualityInspectionCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Notes).MaximumLength(1000);
    }
}
