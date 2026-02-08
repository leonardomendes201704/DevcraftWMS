using FluentValidation;

namespace DevcraftWMS.Application.Features.Warehouses.Commands.UpdateWarehouse;

public sealed class UpdateWarehouseCommandValidator : AbstractValidator<UpdateWarehouseCommand>
{
    public UpdateWarehouseCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Code).NotEmpty().MaximumLength(32);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ShortName).MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(1000);
        RuleFor(x => x.Timezone).MaximumLength(100);
        RuleFor(x => x.ExternalId).MaximumLength(100);
        RuleFor(x => x.ErpCode).MaximumLength(100);
        RuleFor(x => x.CostCenterCode).MaximumLength(50);
        RuleFor(x => x.CostCenterName).MaximumLength(200);

        When(x => x.Address is not null, () =>
        {
            RuleFor(x => x.Address!.AddressLine1).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Address!.AddressNumber).MaximumLength(20);
            RuleFor(x => x.Address!.AddressLine2).MaximumLength(200);
            RuleFor(x => x.Address!.District).MaximumLength(100);
            RuleFor(x => x.Address!.City).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Address!.State).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Address!.PostalCode).NotEmpty().MaximumLength(20);
            RuleFor(x => x.Address!.Country).NotEmpty().MaximumLength(100);
        });

        When(x => x.Contact is not null, () =>
        {
            RuleFor(x => x.Contact!.ContactName).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Contact!.ContactEmail).MaximumLength(200).EmailAddress();
            RuleFor(x => x.Contact!.ContactPhone).MaximumLength(50);
        });
    }
}
