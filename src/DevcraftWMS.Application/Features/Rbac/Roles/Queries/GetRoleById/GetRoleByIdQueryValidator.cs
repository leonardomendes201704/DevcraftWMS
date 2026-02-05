using FluentValidation;

namespace DevcraftWMS.Application.Features.Rbac.Roles.Queries.GetRoleById;

public sealed class GetRoleByIdQueryValidator : AbstractValidator<GetRoleByIdQuery>
{
    public GetRoleByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
