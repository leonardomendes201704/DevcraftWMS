using FluentValidation;

namespace DevcraftWMS.Application.Features.Aisles.Commands.CreateAisle;

public sealed class CreateAisleCommandValidator : AbstractValidator<CreateAisleCommand>
{
    public CreateAisleCommandValidator()
    {
        RuleFor(x => x.SectionId).NotEmpty();
        RuleFor(x => x.Code).NotEmpty().MaximumLength(32);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}
