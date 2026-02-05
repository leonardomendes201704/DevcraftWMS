using FluentValidation;

namespace DevcraftWMS.Application.Features.OutboundChecks.Commands.RegisterOutboundCheck;

public sealed class RegisterOutboundCheckCommandValidator : AbstractValidator<RegisterOutboundCheckCommand>
{
    public RegisterOutboundCheckCommandValidator()
    {
        RuleFor(x => x.OutboundOrderId).NotEmpty();
        RuleFor(x => x.Items).NotEmpty();
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.OutboundOrderItemId).NotEmpty();
            item.RuleFor(i => i.QuantityChecked).GreaterThanOrEqualTo(0);
        });
    }
}
