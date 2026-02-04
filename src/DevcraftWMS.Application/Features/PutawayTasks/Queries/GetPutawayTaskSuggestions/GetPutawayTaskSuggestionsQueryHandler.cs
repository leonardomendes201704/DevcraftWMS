using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;
using MediatR;

namespace DevcraftWMS.Application.Features.PutawayTasks.Queries.GetPutawayTaskSuggestions;

public sealed class GetPutawayTaskSuggestionsQueryHandler
    : IRequestHandler<GetPutawayTaskSuggestionsQuery, RequestResult<IReadOnlyList<PutawaySuggestionDto>>>
{
    private readonly IPutawayTaskRepository _taskRepository;
    private readonly IReceiptRepository _receiptRepository;
    private readonly IProductRepository _productRepository;
    private readonly IStructureRepository _structureRepository;
    private readonly ILocationRepository _locationRepository;

    public GetPutawayTaskSuggestionsQueryHandler(
        IPutawayTaskRepository taskRepository,
        IReceiptRepository receiptRepository,
        IProductRepository productRepository,
        IStructureRepository structureRepository,
        ILocationRepository locationRepository)
    {
        _taskRepository = taskRepository;
        _receiptRepository = receiptRepository;
        _productRepository = productRepository;
        _structureRepository = structureRepository;
        _locationRepository = locationRepository;
    }

    public async Task<RequestResult<IReadOnlyList<PutawaySuggestionDto>>> Handle(GetPutawayTaskSuggestionsQuery request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(request.Id, cancellationToken);
        if (task is null)
        {
            return RequestResult<IReadOnlyList<PutawaySuggestionDto>>.Failure("putaway.task.not_found", "Putaway task not found.");
        }

        var items = await _receiptRepository.ListItemsAsync(task.ReceiptId, null, null, null, null, true, 1, 1, "CreatedAtUtc", "asc", cancellationToken);
        var item = items.FirstOrDefault();
        if (item is null)
        {
            var receipt = await _receiptRepository.GetByIdAsync(task.ReceiptId, cancellationToken);
            item = receipt?.Items
                .OrderBy(i => i.CreatedAtUtc)
                .FirstOrDefault();
        }
        if (item is null)
        {
            return RequestResult<IReadOnlyList<PutawaySuggestionDto>>.Success(Array.Empty<PutawaySuggestionDto>());
        }

        var product = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken);
        if (product is null)
        {
            return RequestResult<IReadOnlyList<PutawaySuggestionDto>>.Success(Array.Empty<PutawaySuggestionDto>());
        }

        var structures = await _structureRepository.ListByWarehouseAsync(task.WarehouseId, true, false, cancellationToken);
        var candidateLocations = new List<Location>();
        foreach (var structure in structures)
        {
            var locations = await _locationRepository.ListByStructureAsync(structure.Id, null, true, false, cancellationToken);
            candidateLocations.AddRange(locations);
        }

        var suggestions = candidateLocations
            .Where(location => location.Zone?.ZoneType != ZoneType.Quarantine)
            .Select(location => BuildSuggestion(location, product, item.Quantity))
            .Where(result => result.IsCompatible)
            .OrderBy(result => result.Score)
            .ThenBy(result => result.Location.Code)
            .Take(Math.Max(1, request.Limit))
            .Select(result => new PutawaySuggestionDto(
                result.Location.Id,
                result.Location.Code,
                result.Location.Zone?.Name ?? "-",
                result.Location.Zone?.ZoneType,
                result.Score,
                result.Reason))
            .ToList();

        if (suggestions.Count == 0)
        {
            var fallbackLocation = item.Location ?? await _locationRepository.GetByIdAsync(item.LocationId, cancellationToken);
            if (fallbackLocation is not null && fallbackLocation.Zone?.ZoneType != ZoneType.Quarantine)
            {
                var fallback = BuildSuggestion(fallbackLocation, product, item.Quantity);
                if (fallback.IsCompatible)
                {
                    suggestions.Add(new PutawaySuggestionDto(
                        fallback.Location.Id,
                        fallback.Location.Code,
                        fallback.Location.Zone?.Name ?? "-",
                        fallback.Location.Zone?.ZoneType,
                        fallback.Score,
                        "Fallback to receipt location."));
                }
            }
        }

        return RequestResult<IReadOnlyList<PutawaySuggestionDto>>.Success(suggestions);
    }

    private static SuggestionResult BuildSuggestion(Location location, Product product, decimal quantity)
    {
        var compatibility = ValidateCompatibility(location, product, quantity);
        if (!compatibility.IsCompatible)
        {
            return compatibility;
        }

        var zoneType = location.Zone?.ZoneType;
        var score = zoneType switch
        {
            ZoneType.Storage => 0,
            ZoneType.Bulk => 1,
            ZoneType.Picking => 2,
            ZoneType.Staging => 3,
            ZoneType.CrossDock => 4,
            _ => 5
        };

        var reason = $"Compatible location. Zone: {(zoneType?.ToString() ?? "Unassigned")}.";
        return compatibility with { Score = score, Reason = reason };
    }

    private static SuggestionResult ValidateCompatibility(Location location, Product product, decimal quantity)
    {
        if (product.TrackingMode is TrackingMode.Lot or TrackingMode.LotAndExpiry && !location.AllowLotTracking)
        {
            return SuggestionResult.Incompatible(location, "Lot tracking not allowed.");
        }

        if (product.TrackingMode == TrackingMode.LotAndExpiry && !location.AllowExpiryTracking)
        {
            return SuggestionResult.Incompatible(location, "Expiry tracking not allowed.");
        }

        if (location.MaxWeightKg.HasValue && product.WeightKg.HasValue)
        {
            var totalWeight = product.WeightKg.Value * quantity;
            if (totalWeight > location.MaxWeightKg.Value)
            {
                return SuggestionResult.Incompatible(location, "Weight capacity exceeded.");
            }
        }

        if (location.MaxVolumeM3.HasValue && product.VolumeCm3.HasValue)
        {
            var volumeM3 = product.VolumeCm3.Value / 1_000_000m;
            var totalVolume = volumeM3 * quantity;
            if (totalVolume > location.MaxVolumeM3.Value)
            {
                return SuggestionResult.Incompatible(location, "Volume capacity exceeded.");
            }
        }

        return SuggestionResult.Compatible(location);
    }

    private sealed record SuggestionResult(Location Location, bool IsCompatible, int Score, string Reason)
    {
        public static SuggestionResult Compatible(Location location)
            => new(location, true, 0, "Compatible location.");

        public static SuggestionResult Incompatible(Location location, string reason)
            => new(location, false, int.MaxValue, reason);
    }
}
