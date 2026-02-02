using FluentValidation;

namespace DevcraftWMS.Application.Features.Customers.Queries.ListCustomersPaged;

public sealed class ListCustomersPagedQueryValidator : AbstractValidator<ListCustomersPagedQuery>
{
    private const int MaxPageSize = 100;

    public ListCustomersPagedQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("PageNumber must be greater than zero.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(MaxPageSize)
            .WithMessage($"PageSize must be between 1 and {MaxPageSize}.");
    }
}
