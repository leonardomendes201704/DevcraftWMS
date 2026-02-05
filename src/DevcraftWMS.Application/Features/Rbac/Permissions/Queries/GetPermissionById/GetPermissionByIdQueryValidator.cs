using FluentValidation;

namespace DevcraftWMS.Application.Features.Rbac.Permissions.Queries.GetPermissionById;

public sealed class GetPermissionByIdQueryValidator : AbstractValidator<GetPermissionByIdQuery>
{
    public GetPermissionByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
