using FluentValidation;

namespace PerfectApiTemplate.Application.Features.Emails.Commands.EnqueueEmail;

public sealed class EnqueueEmailCommandValidator : AbstractValidator<EnqueueEmailCommand>
{
    public EnqueueEmailCommandValidator()
    {
        RuleFor(x => x.To).NotEmpty().EmailAddress();
        RuleFor(x => x.Subject).NotEmpty().MaximumLength(998);
        RuleFor(x => x.Body).NotEmpty();
        RuleFor(x => x.From).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.From));
    }
}

