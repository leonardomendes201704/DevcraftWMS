using FluentValidation;

namespace DevcraftWMS.Application.Features.Structures.Commands.UpdateStructure;

public sealed class UpdateStructureCommandValidator : AbstractValidator<UpdateStructureCommand>
{
    public UpdateStructureCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.SectionId).NotEmpty();
        RuleFor(x => x.Code).NotEmpty().MaximumLength(32);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Levels).GreaterThan(0).LessThanOrEqualTo(200);
    }
}
