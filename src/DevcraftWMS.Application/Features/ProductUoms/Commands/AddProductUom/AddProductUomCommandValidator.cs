using FluentValidation;

namespace DevcraftWMS.Application.Features.ProductUoms.Commands.AddProductUom;

public sealed class AddProductUomCommandValidator : AbstractValidator<AddProductUomCommand>
{
    public AddProductUomCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.UomId).NotEmpty();
        RuleFor(x => x.ConversionFactor).GreaterThan(0);
    }
}
