using FluentValidation;

namespace DevcraftWMS.Application.Features.UnitLoads.Queries.GetUnitLoadById;

public sealed class GetUnitLoadByIdQueryValidator : AbstractValidator<GetUnitLoadByIdQuery>
{
    public GetUnitLoadByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
