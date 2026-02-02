using FluentValidation;

namespace DevcraftWMS.Application.Features.Emails.Queries.ListEmails;

public sealed class ListEmailsQueryValidator : AbstractValidator<ListEmailsQuery>
{
    private const int MaxPageSize = 100;

    public ListEmailsQueryValidator()
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
