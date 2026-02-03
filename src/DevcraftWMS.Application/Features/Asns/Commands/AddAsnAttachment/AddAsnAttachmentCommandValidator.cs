using FluentValidation;

namespace DevcraftWMS.Application.Features.Asns.Commands.AddAsnAttachment;

public sealed class AddAsnAttachmentCommandValidator : AbstractValidator<AddAsnAttachmentCommand>
{
    public AddAsnAttachmentCommandValidator()
    {
        RuleFor(x => x.AsnId)
            .NotEmpty()
            .WithMessage("AsnId is required.");

        RuleFor(x => x.FileName)
            .NotEmpty()
            .WithMessage("FileName is required.");

        RuleFor(x => x.Content)
            .NotNull()
            .Must(c => c.Length > 0)
            .WithMessage("Attachment content is required.");
    }
}
