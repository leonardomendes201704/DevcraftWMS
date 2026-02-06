using FluentValidation;

namespace DevcraftWMS.Application.Features.InventoryVisibility.Queries.ExportInventoryVisibility;

public sealed class ExportInventoryVisibilityQueryValidator : AbstractValidator<ExportInventoryVisibilityQuery>
{
    public ExportInventoryVisibilityQueryValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty();

        RuleFor(x => x.WarehouseId)
            .NotEmpty();

        RuleFor(x => x.Format)
            .NotEmpty()
            .Must(format => format is "pdf" or "xlsx" or "print")
            .WithMessage("Format must be pdf, xlsx, or print.");

        RuleFor(x => x.OrderBy)
            .NotEmpty();

        RuleFor(x => x.OrderDir)
            .NotEmpty();
    }
}
