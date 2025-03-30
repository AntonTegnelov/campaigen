using Campaigen.Core.Application.Features.SpendTracking.Abstractions;
using Campaigen.Core.Application.Features.SpendTracking.DTOs;
using Campaigen.Core.Domain.Features.SpendTracking;
using System.Linq;

namespace Campaigen.Core.Application.Features.SpendTracking;

/// <summary>
/// Service implementation for managing spend tracking operations.
/// </summary>
public class SpendTrackingService : ISpendTrackingService
{
    private readonly ISpendTrackingRepository _spendTrackingRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpendTrackingService"/> class.
    /// </summary>
    /// <param name="spendTrackingRepository">The spend tracking repository dependency.</param>
    public SpendTrackingService(ISpendTrackingRepository spendTrackingRepository)
    {
        _spendTrackingRepository = spendTrackingRepository;
    }

    /// <inheritdoc />
    public async Task<SpendRecordDto?> CreateSpendRecordAsync(CreateSpendRecordDto dto)
    {
        // Map DTO to domain entity
        var spendRecord = new SpendRecord
        {
            Id = Guid.NewGuid(), // Generate ID here
            Date = dto.Date, // Consider UTC normalization if needed
            Amount = dto.Amount,
            Description = dto.Description,
            Category = dto.Category
        };

        // Persist using the repository
        await _spendTrackingRepository.AddAsync(spendRecord);

        // Map the created entity back to a DTO to return
        return MapToDto(spendRecord);
    }

    /// <inheritdoc />
    public async Task<SpendRecordDto?> GetSpendRecordAsync(Guid id)
    {
        var spendRecord = await _spendTrackingRepository.GetByIdAsync(id);
        return spendRecord == null ? null : MapToDto(spendRecord);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<SpendRecordDto>> ListSpendRecordsAsync()
    {
        var spendRecords = await _spendTrackingRepository.GetAllAsync();
        // Use Select with the mapping method
        return spendRecords.Select(MapToDto);
    }

    /// <summary>
    /// Maps a <see cref="SpendRecord"/> domain entity to a <see cref="SpendRecordDto"/>.
    /// </summary>
    /// <param name="spendRecord">The domain entity to map.</param>
    /// <returns>The resulting DTO.</returns>
    private static SpendRecordDto MapToDto(SpendRecord spendRecord)
    {
        return new SpendRecordDto
        {
            Id = spendRecord.Id,
            Date = spendRecord.Date,
            Amount = spendRecord.Amount,
            Description = spendRecord.Description,
            Category = spendRecord.Category
        };
    }
} 