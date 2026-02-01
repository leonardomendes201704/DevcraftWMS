using FluentValidation;

namespace DevcraftWMS.Application.Features.Structures.Commands.CreateStructure;

public sealed class CreateStructureCommandValidator : AbstractValidator<CreateStructureCommand>
{
    public CreateStructureCommandValidator()
    {
        RuleFor(x => x.SectionId).NotEmpty();
        RuleFor(x => x.Code).NotEmpty().MaximumLength(32);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Levels).GreaterThan(0).LessThanOrEqualTo(200);
    }
}
