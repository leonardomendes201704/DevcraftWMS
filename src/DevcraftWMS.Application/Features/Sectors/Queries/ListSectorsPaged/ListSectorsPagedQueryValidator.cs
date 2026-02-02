using FluentValidation;

namespace DevcraftWMS.Application.Features.Sectors.Queries.ListSectorsPaged;

public sealed class ListSectorsPagedQueryValidator : AbstractValidator<ListSectorsPagedQuery>
{
    private const int MaxPageSize = 100;

    public ListSectorsPagedQueryValidator()
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
