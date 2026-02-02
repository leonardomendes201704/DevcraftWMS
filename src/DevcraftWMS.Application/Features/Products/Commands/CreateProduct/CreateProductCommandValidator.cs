using FluentValidation;

namespace DevcraftWMS.Application.Features.Products.Commands.CreateProduct;

public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(32);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(2000);
        RuleFor(x => x.Ean).MaximumLength(50);
        RuleFor(x => x.ErpCode).MaximumLength(50);
        RuleFor(x => x.Category).MaximumLength(100);
        RuleFor(x => x.Brand).MaximumLength(100);
        RuleFor(x => x.BaseUomId).NotEmpty();
        RuleFor(x => x.TrackingMode).Must(mode => Enum.IsDefined(mode))
            .WithMessage("Tracking mode is invalid.");
        RuleFor(x => x.WeightKg).GreaterThanOrEqualTo(0).When(x => x.WeightKg.HasValue);
        RuleFor(x => x.LengthCm).GreaterThan(0).When(x => x.LengthCm.HasValue);
        RuleFor(x => x.WidthCm).GreaterThan(0).When(x => x.WidthCm.HasValue);
        RuleFor(x => x.HeightCm).GreaterThan(0).When(x => x.HeightCm.HasValue);
        RuleFor(x => x.VolumeCm3).GreaterThan(0).When(x => x.VolumeCm3.HasValue);
    }
}
