using FluentValidation;

namespace DevcraftWMS.Application.Features.Rbac.Permissions.Commands.DeactivatePermission;

public sealed class DeactivatePermissionCommandValidator : AbstractValidator<DeactivatePermissionCommand>
{
    public DeactivatePermissionCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
