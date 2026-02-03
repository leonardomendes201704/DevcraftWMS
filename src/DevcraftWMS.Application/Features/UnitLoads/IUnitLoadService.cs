using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;

namespace DevcraftWMS.Application.Features.UnitLoads;

public interface IUnitLoadService
{
    Task<RequestResult<UnitLoadDetailDto>> CreateAsync(
        Guid receiptId,
        string? ssccExternal,
        string? notes,
        CancellationToken cancellationToken);
    Task<RequestResult<UnitLoadDetailDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<RequestResult<PagedResult<UnitLoadListItemDto>>> ListAsync(
        Guid? warehouseId,
        Guid? receiptId,
        string? sscc,
        DevcraftWMS.Domain.Enums.UnitLoadStatus? status,
        bool? isActive,
        bool includeInactive,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken);
    Task<RequestResult<UnitLoadLabelDto>> PrintLabelAsync(Guid id, CancellationToken cancellationToken);
}
