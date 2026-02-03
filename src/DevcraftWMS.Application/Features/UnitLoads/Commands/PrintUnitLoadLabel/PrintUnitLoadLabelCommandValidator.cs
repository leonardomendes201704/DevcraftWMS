using FluentValidation;

namespace DevcraftWMS.Application.Features.UnitLoads.Commands.PrintUnitLoadLabel;

public sealed class PrintUnitLoadLabelCommandValidator : AbstractValidator<PrintUnitLoadLabelCommand>
{
    public PrintUnitLoadLabelCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
