using FluentValidation;

namespace DevcraftWMS.Application.Features.Aisles.Commands.UpdateAisle;

public sealed class UpdateAisleCommandValidator : AbstractValidator<UpdateAisleCommand>
{
    public UpdateAisleCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.SectionId).NotEmpty();
        RuleFor(x => x.Code).NotEmpty().MaximumLength(32);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}
