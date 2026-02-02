using FluentValidation;

namespace DevcraftWMS.Application.Features.Sections.Queries.ListSectionsPaged;

public sealed class ListSectionsPagedQueryValidator : AbstractValidator<ListSectionsPagedQuery>
{
    public ListSectionsPagedQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("PageNumber must be greater than zero.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("PageSize must be greater than zero.");
    }
}
