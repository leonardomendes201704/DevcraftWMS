using FluentValidation;

namespace DevcraftWMS.Application.Features.ReceiptDivergences.Commands.AddReceiptDivergenceEvidence;

public sealed class AddReceiptDivergenceEvidenceCommandValidator : AbstractValidator<AddReceiptDivergenceEvidenceCommand>
{
    public AddReceiptDivergenceEvidenceCommandValidator()
    {
        RuleFor(x => x.DivergenceId).NotEmpty();
        RuleFor(x => x.Evidence).NotNull();
        RuleFor(x => x.Evidence.FileName).NotEmpty().MaximumLength(260);
        RuleFor(x => x.Evidence.ContentType).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Evidence.SizeBytes).GreaterThan(0);
        RuleFor(x => x.Evidence.Content).NotNull();
    }
}
