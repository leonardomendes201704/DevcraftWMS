using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.Asns;

public sealed class AsnService : IAsnService
{
    private readonly IAsnRepository _asnRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly ICustomerContext _customerContext;

    public AsnService(
        IAsnRepository asnRepository,
        IWarehouseRepository warehouseRepository,
        ICustomerContext customerContext)
    {
        _asnRepository = asnRepository;
        _warehouseRepository = warehouseRepository;
        _customerContext = customerContext;
    }

    public async Task<RequestResult<AsnDetailDto>> CreateAsync(
        Guid warehouseId,
        string asnNumber,
        string? documentNumber,
        string? supplierName,
        DateOnly? expectedArrivalDate,
        string? notes,
        CancellationToken cancellationToken)
    {
        if (!_customerContext.CustomerId.HasValue)
        {
            return RequestResult<AsnDetailDto>.Failure("customers.context.required", "Customer context is required.");
        }

        if (warehouseId == Guid.Empty)
        {
            return RequestResult<AsnDetailDto>.Failure("asns.warehouse.required", "Warehouse is required.");
        }

        var warehouse = await _warehouseRepository.GetByIdAsync(warehouseId, cancellationToken);
        if (warehouse is null)
        {
            return RequestResult<AsnDetailDto>.Failure("asns.warehouse.not_found", "Warehouse not found.");
        }

        var numberExists = await _asnRepository.AsnNumberExistsAsync(asnNumber, cancellationToken);
        if (numberExists)
        {
            return RequestResult<AsnDetailDto>.Failure("asns.asn.number_exists", "ASN number already exists.");
        }

        var asn = new Asn
        {
            Id = Guid.NewGuid(),
            CustomerId = _customerContext.CustomerId.Value,
            WarehouseId = warehouseId,
            AsnNumber = asnNumber,
            DocumentNumber = documentNumber,
            SupplierName = supplierName,
            ExpectedArrivalDate = expectedArrivalDate,
            Notes = notes,
            Status = AsnStatus.Registered
        };

        await _asnRepository.AddAsync(asn, cancellationToken);
        asn.Warehouse = warehouse;
        return RequestResult<AsnDetailDto>.Success(AsnMapping.MapDetail(asn));
    }

    public async Task<RequestResult<AsnDetailDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var asn = await _asnRepository.GetByIdAsync(id, cancellationToken);
        if (asn is null)
        {
            return RequestResult<AsnDetailDto>.Failure("asns.asn.not_found", "ASN not found.");
        }

        return RequestResult<AsnDetailDto>.Success(AsnMapping.MapDetail(asn));
    }

    public async Task<RequestResult<PagedResult<AsnListItemDto>>> ListAsync(
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
        CancellationToken cancellationToken)
    {
        var total = await _asnRepository.CountAsync(
            warehouseId,
            asnNumber,
            supplierName,
            documentNumber,
            status,
            expectedFrom,
            expectedTo,
            isActive,
            includeInactive,
            cancellationToken);

        var items = await _asnRepository.ListAsync(
            warehouseId,
            pageNumber,
            pageSize,
            orderBy,
            orderDir,
            asnNumber,
            supplierName,
            documentNumber,
            status,
            expectedFrom,
            expectedTo,
            isActive,
            includeInactive,
            cancellationToken);

        var mapped = items.Select(AsnMapping.MapListItem).ToList();
        var result = new PagedResult<AsnListItemDto>(mapped, total, pageNumber, pageSize, orderBy, orderDir);
        return RequestResult<PagedResult<AsnListItemDto>>.Success(result);
    }
}
