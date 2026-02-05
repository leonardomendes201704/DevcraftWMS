using FluentValidation;

namespace DevcraftWMS.Application.Features.Rbac.Permissions.Queries.ListPermissions;

public sealed class ListPermissionsQueryValidator : AbstractValidator<ListPermissionsQuery>
{
    public ListPermissionsQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.OrderBy).NotEmpty();
        RuleFor(x => x.OrderDir).NotEmpty();
    }
}
