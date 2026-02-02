using FluentValidation;

namespace DevcraftWMS.Application.Features.Locations.Commands.UpdateLocation;

public sealed class UpdateLocationCommandValidator : AbstractValidator<UpdateLocationCommand>
{
    public UpdateLocationCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.StructureId).NotEmpty();
        RuleFor(x => x.Code).NotEmpty().MaximumLength(32);
        RuleFor(x => x.Barcode).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Level).GreaterThan(0).LessThanOrEqualTo(200);
        RuleFor(x => x.Row).GreaterThan(0).LessThanOrEqualTo(200);
        RuleFor(x => x.Column).GreaterThan(0).LessThanOrEqualTo(200);
    }
}
