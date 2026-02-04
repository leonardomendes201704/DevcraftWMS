using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.QualityInspections.Commands.ApproveQualityInspection;

public sealed class ApproveQualityInspectionCommandHandler : IRequestHandler<ApproveQualityInspectionCommand, RequestResult<QualityInspectionDetailDto>>
{
    private readonly IQualityInspectionService _service;

    public ApproveQualityInspectionCommandHandler(IQualityInspectionService service)
    {
        _service = service;
    }

    public Task<RequestResult<QualityInspectionDetailDto>> Handle(ApproveQualityInspectionCommand request, CancellationToken cancellationToken)
        => _service.ApproveAsync(request.Id, request.Notes, cancellationToken);
}
