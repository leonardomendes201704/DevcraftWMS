using FluentValidation;

namespace DevcraftWMS.Application.Features.Logs.Queries.ListErrorLogs;

public sealed class ListErrorLogsQueryValidator : AbstractValidator<ListErrorLogsQuery>
{
    private const int MaxPageSize = 200;

    public ListErrorLogsQueryValidator()
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
