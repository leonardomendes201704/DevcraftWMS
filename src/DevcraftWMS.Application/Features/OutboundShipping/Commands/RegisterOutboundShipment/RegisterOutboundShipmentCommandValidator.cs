using FluentValidation;

namespace DevcraftWMS.Application.Features.OutboundShipping.Commands.RegisterOutboundShipment;

public sealed class RegisterOutboundShipmentCommandValidator : AbstractValidator<RegisterOutboundShipmentCommand>
{
    public RegisterOutboundShipmentCommandValidator()
    {
        RuleFor(x => x.OutboundOrderId).NotEmpty();
        RuleFor(x => x.Input.DockCode).NotEmpty();
        RuleFor(x => x.Input.Packages).NotEmpty();
        RuleForEach(x => x.Input.Packages).ChildRules(item =>
        {
            item.RuleFor(i => i.OutboundPackageId).NotEmpty();
        });
    }
}
