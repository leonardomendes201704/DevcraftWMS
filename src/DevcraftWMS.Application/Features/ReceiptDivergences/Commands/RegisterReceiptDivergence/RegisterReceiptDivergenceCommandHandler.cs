using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.ReceiptDivergences.Commands.RegisterReceiptDivergence;

public sealed class RegisterReceiptDivergenceCommandHandler : IRequestHandler<RegisterReceiptDivergenceCommand, RequestResult<ReceiptDivergenceDto>>
{
    private readonly IReceiptDivergenceService _service;

    public RegisterReceiptDivergenceCommandHandler(IReceiptDivergenceService service)
    {
        _service = service;
    }

    public Task<RequestResult<ReceiptDivergenceDto>> Handle(RegisterReceiptDivergenceCommand request, CancellationToken cancellationToken)
        => _service.RegisterAsync(
            request.ReceiptId,
            request.InboundOrderItemId,
            request.Type,
            request.Notes,
            request.Evidence,
            cancellationToken);
}
