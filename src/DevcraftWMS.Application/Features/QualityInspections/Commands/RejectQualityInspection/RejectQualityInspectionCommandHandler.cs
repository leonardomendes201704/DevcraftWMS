using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.QualityInspections.Commands.RejectQualityInspection;

public sealed class RejectQualityInspectionCommandHandler : IRequestHandler<RejectQualityInspectionCommand, RequestResult<QualityInspectionDetailDto>>
{
    private readonly IQualityInspectionService _service;

    public RejectQualityInspectionCommandHandler(IQualityInspectionService service)
    {
        _service = service;
    }

    public Task<RequestResult<QualityInspectionDetailDto>> Handle(RejectQualityInspectionCommand request, CancellationToken cancellationToken)
        => _service.RejectAsync(request.Id, request.Notes, cancellationToken);
}
