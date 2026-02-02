using FluentValidation;

namespace DevcraftWMS.Application.Features.Zones.Commands.CreateZone;

public sealed class CreateZoneCommandValidator : AbstractValidator<CreateZoneCommand>
{
    public CreateZoneCommandValidator()
    {
        RuleFor(x => x.WarehouseId).NotEmpty();
        RuleFor(x => x.Code).NotEmpty().MaximumLength(32);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.ZoneType).Must(mode => Enum.IsDefined(mode))
            .WithMessage("Zone type is invalid.");
    }
}
