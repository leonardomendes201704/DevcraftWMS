using FluentValidation;

namespace DevcraftWMS.Application.Features.OutboundPacking.Commands.RegisterOutboundPacking;

public sealed class RegisterOutboundPackingCommandValidator : AbstractValidator<RegisterOutboundPackingCommand>
{
    public RegisterOutboundPackingCommandValidator()
    {
        RuleFor(x => x.OutboundOrderId).NotEmpty();
        RuleFor(x => x.Packages).NotEmpty();
        RuleForEach(x => x.Packages).ChildRules(package =>
        {
            package.RuleFor(p => p.PackageNumber).NotEmpty();
            package.RuleFor(p => p.Items).NotEmpty();
            package.RuleForEach(p => p.Items).ChildRules(item =>
            {
                item.RuleFor(i => i.OutboundOrderItemId).NotEmpty();
                item.RuleFor(i => i.Quantity).GreaterThan(0);
            });
        });
    }
}
