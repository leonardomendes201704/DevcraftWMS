using FluentValidation;

namespace DevcraftWMS.Application.Features.Asns.Commands.AddAsnItem;

public sealed class AddAsnItemCommandValidator : AbstractValidator<AddAsnItemCommand>
{
    public AddAsnItemCommandValidator()
    {
        RuleFor(x => x.AsnId).NotEmpty();
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.UomId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}
