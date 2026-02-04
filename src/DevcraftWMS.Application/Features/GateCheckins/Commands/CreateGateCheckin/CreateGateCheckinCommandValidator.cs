using FluentValidation;

namespace DevcraftWMS.Application.Features.GateCheckins.Commands.CreateGateCheckin;

public sealed class CreateGateCheckinCommandValidator : AbstractValidator<CreateGateCheckinCommand>
{
    public CreateGateCheckinCommandValidator()
    {
        RuleFor(x => x.VehiclePlate)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.DriverName)
            .NotEmpty()
            .MaximumLength(120);

        RuleFor(x => x.CarrierName)
            .MaximumLength(120);

        RuleFor(x => x.DocumentNumber)
            .MaximumLength(64);

        RuleFor(x => x.Notes)
            .MaximumLength(500);

        RuleFor(x => x.InboundOrderId)
            .Must(id => !id.HasValue || id.Value != Guid.Empty)
            .WithMessage("InboundOrderId cannot be empty.");

        RuleFor(x => x)
            .Must(HasInboundOrderOrDocument)
            .WithMessage("InboundOrderId or DocumentNumber is required.");

        RuleFor(x => x.WarehouseId)
            .Must(id => !id.HasValue || id.Value != Guid.Empty)
            .WithMessage("WarehouseId cannot be empty.");

        RuleFor(x => x)
            .Must(HasWarehouseForEmergency)
            .WithMessage("WarehouseId is required to create an emergency inbound order.");
    }

    private static bool HasInboundOrderOrDocument(CreateGateCheckinCommand command)
    {
        var hasInboundOrder = command.InboundOrderId.HasValue && command.InboundOrderId.Value != Guid.Empty;
        var hasDocument = !string.IsNullOrWhiteSpace(command.DocumentNumber);
        return hasInboundOrder || hasDocument;
    }

    private static bool HasWarehouseForEmergency(CreateGateCheckinCommand command)
    {
        var hasInboundOrder = command.InboundOrderId.HasValue && command.InboundOrderId.Value != Guid.Empty;
        if (hasInboundOrder)
        {
            return true;
        }

        var hasDocument = !string.IsNullOrWhiteSpace(command.DocumentNumber);
        if (!hasDocument)
        {
            return true;
        }

        return command.WarehouseId.HasValue && command.WarehouseId.Value != Guid.Empty;
    }
}
