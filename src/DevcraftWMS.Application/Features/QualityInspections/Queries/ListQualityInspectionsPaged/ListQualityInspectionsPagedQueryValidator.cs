using FluentValidation;

namespace DevcraftWMS.Application.Features.QualityInspections.Queries.ListQualityInspectionsPaged;

public sealed class ListQualityInspectionsPagedQueryValidator : AbstractValidator<ListQualityInspectionsPagedQuery>
{
    public ListQualityInspectionsPagedQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.OrderBy).NotEmpty();
        RuleFor(x => x.OrderDir).NotEmpty();
    }
}
