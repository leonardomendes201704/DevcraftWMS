using FluentValidation;

namespace DevcraftWMS.Application.Features.PickingReplenishments.Commands.GeneratePickingReplenishments;

public sealed class GeneratePickingReplenishmentsCommandValidator : AbstractValidator<GeneratePickingReplenishmentsCommand>
{
    public GeneratePickingReplenishmentsCommandValidator()
    {
        RuleFor(x => x.WarehouseId).NotEqual(Guid.Empty).When(x => x.WarehouseId.HasValue);
    }
}
