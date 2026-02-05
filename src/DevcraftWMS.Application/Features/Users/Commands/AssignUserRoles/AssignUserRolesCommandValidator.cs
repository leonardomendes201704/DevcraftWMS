using FluentValidation;

namespace DevcraftWMS.Application.Features.Users.Commands.AssignUserRoles;

public sealed class AssignUserRolesCommandValidator : AbstractValidator<AssignUserRolesCommand>
{
    public AssignUserRolesCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.RoleIds).NotNull();
    }
}
