using FluentValidation;

namespace DevcraftWMS.Application.Features.InboundOrders.Commands.UpdateInboundOrderParameters;

public sealed class UpdateInboundOrderParametersCommandValidator : AbstractValidator<UpdateInboundOrderParametersCommand>
{
    public UpdateInboundOrderParametersCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.SuggestedDock).MaximumLength(32);
    }
}
