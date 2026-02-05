using FluentValidation;

namespace DevcraftWMS.Application.Features.Rbac.Roles.Commands.DeactivateRole;

public sealed class DeactivateRoleCommandValidator : AbstractValidator<DeactivateRoleCommand>
{
    public DeactivateRoleCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
