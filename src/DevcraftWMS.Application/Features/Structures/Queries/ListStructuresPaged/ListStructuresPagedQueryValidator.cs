using FluentValidation;

namespace DevcraftWMS.Application.Features.Structures.Queries.ListStructuresPaged;

public sealed class ListStructuresPagedQueryValidator : AbstractValidator<ListStructuresPagedQuery>
{
    public ListStructuresPagedQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("PageNumber must be greater than zero.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("PageSize must be greater than zero.");
    }
}
