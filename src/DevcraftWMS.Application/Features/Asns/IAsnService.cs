using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.Asns;

public interface IAsnService
{
    Task<RequestResult<AsnDetailDto>> CreateAsync(
        Guid warehouseId,
        string asnNumber,
        string? documentNumber,
        string? supplierName,
        DateOnly? expectedArrivalDate,
        string? notes,
        CancellationToken cancellationToken);

    Task<RequestResult<AsnDetailDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<RequestResult<PagedResult<AsnListItemDto>>> ListAsync(
        Guid? warehouseId,
        string? asnNumber,
        string? supplierName,
        string? documentNumber,
        AsnStatus? status,
        DateOnly? expectedFrom,
        DateOnly? expectedTo,
        bool? isActive,
        bool includeInactive,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken);
}
