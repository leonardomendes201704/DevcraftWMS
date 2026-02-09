using FluentValidation;

namespace DevcraftWMS.Application.Features.Asns.Commands.UpdateAsn;

public sealed class UpdateAsnCommandValidator : AbstractValidator<UpdateAsnCommand>
{
    public UpdateAsnCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.WarehouseId)
            .NotEmpty();

        RuleFor(x => x.AsnNumber)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.DocumentNumber)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.DocumentNumber));

        RuleFor(x => x.SupplierName)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.SupplierName));

        RuleFor(x => x.Notes)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}
