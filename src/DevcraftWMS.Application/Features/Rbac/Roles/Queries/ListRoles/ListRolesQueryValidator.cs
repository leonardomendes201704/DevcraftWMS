using FluentValidation;

namespace DevcraftWMS.Application.Features.Rbac.Roles.Queries.ListRoles;

public sealed class ListRolesQueryValidator : AbstractValidator<ListRolesQuery>
{
    public ListRolesQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.OrderBy).NotEmpty();
        RuleFor(x => x.OrderDir).NotEmpty();
    }
}
