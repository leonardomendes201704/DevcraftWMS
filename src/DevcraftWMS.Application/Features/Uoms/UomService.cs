using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.Uoms;

public sealed class UomService : IUomService
{
    private readonly IUomRepository _uomRepository;

    public UomService(IUomRepository uomRepository)
    {
        _uomRepository = uomRepository;
    }

    public async Task<RequestResult<UomDto>> CreateUomAsync(string code, string name, UomType type, CancellationToken cancellationToken)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();
        if (await _uomRepository.CodeExistsAsync(normalizedCode, cancellationToken))
        {
            return RequestResult<UomDto>.Failure("uoms.uom.code_exists", "A unit of measure with this code already exists.");
        }

        var uom = new Uom
        {
            Id = Guid.NewGuid(),
            Code = normalizedCode,
            Name = name.Trim(),
            Type = type
        };

        await _uomRepository.AddAsync(uom, cancellationToken);
        return RequestResult<UomDto>.Success(UomMapping.Map(uom));
    }

    public async Task<RequestResult<UomDto>> UpdateUomAsync(Guid id, string code, string name, UomType type, CancellationToken cancellationToken)
    {
        var uom = await _uomRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (uom is null)
        {
            return RequestResult<UomDto>.Failure("uoms.uom.not_found", "Unit of measure not found.");
        }

        var normalizedCode = code.Trim().ToUpperInvariant();
        if (!string.Equals(uom.Code, normalizedCode, StringComparison.OrdinalIgnoreCase))
        {
            if (await _uomRepository.CodeExistsAsync(normalizedCode, id, cancellationToken))
            {
                return RequestResult<UomDto>.Failure("uoms.uom.code_exists", "A unit of measure with this code already exists.");
            }
        }

        uom.Code = normalizedCode;
        uom.Name = name.Trim();
        uom.Type = type;

        await _uomRepository.UpdateAsync(uom, cancellationToken);
        return RequestResult<UomDto>.Success(UomMapping.Map(uom));
    }

    public async Task<RequestResult<UomDto>> DeactivateUomAsync(Guid id, CancellationToken cancellationToken)
    {
        var uom = await _uomRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (uom is null)
        {
            return RequestResult<UomDto>.Failure("uoms.uom.not_found", "Unit of measure not found.");
        }

        if (!uom.IsActive)
        {
            return RequestResult<UomDto>.Success(UomMapping.Map(uom));
        }

        uom.IsActive = false;
        await _uomRepository.UpdateAsync(uom, cancellationToken);
        return RequestResult<UomDto>.Success(UomMapping.Map(uom));
    }
}
