using FluentValidation;

namespace DevcraftWMS.Application.Features.InboundOrders.Commands.ApproveEmergencyInboundOrder;

public sealed class ApproveEmergencyInboundOrderCommandValidator : AbstractValidator<ApproveEmergencyInboundOrderCommand>
{
    public ApproveEmergencyInboundOrderCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Notes)
            .MaximumLength(500);
    }
}
