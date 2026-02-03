using FluentValidation;

namespace DevcraftWMS.Application.Features.GateCheckins.Queries.ListGateCheckins;

public sealed class ListGateCheckinsQueryValidator : AbstractValidator<ListGateCheckinsQuery>
{
    public ListGateCheckinsQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 200);
        RuleFor(x => x.OrderBy).NotEmpty();
        RuleFor(x => x.OrderDir).NotEmpty();
        RuleFor(x => x.ArrivalFromUtc)
            .LessThanOrEqualTo(x => x.ArrivalToUtc)
            .When(x => x.ArrivalFromUtc.HasValue && x.ArrivalToUtc.HasValue)
            .WithMessage("ArrivalFromUtc cannot be later than ArrivalToUtc.");
    }
}
