using Campaigen.Core.Application.Features.SpendTracking.Abstractions;
using Campaigen.Core.Application.Features.SpendTracking.DTOs;
using Campaigen.Core.Domain.Features.SpendTracking;
using System.Linq;

namespace Campaigen.Core.Application.Features.SpendTracking;

public class SpendTrackingService : ISpendTrackingService
{
    private readonly ISpendTrackingRepository _spendTrackingRepository;

    public SpendTrackingService(ISpendTrackingRepository spendTrackingRepository)
    {
        _spendTrackingRepository = spendTrackingRepository;
    }

    public async Task<SpendRecordDto?> CreateSpendRecordAsync(CreateSpendRecordDto dto)
    {
        var spendRecord = new SpendRecord
        {
            Id = Guid.NewGuid(),
            Date = dto.Date,
            Amount = dto.Amount,
            Description = dto.Description,
            Category = dto.Category
        };

        await _spendTrackingRepository.AddAsync(spendRecord);

        return MapToDto(spendRecord);
    }

    public async Task<SpendRecordDto?> GetSpendRecordAsync(Guid id)
    {
        var spendRecord = await _spendTrackingRepository.GetByIdAsync(id);
        return spendRecord == null ? null : MapToDto(spendRecord);
    }

    public async Task<IEnumerable<SpendRecordDto>> ListSpendRecordsAsync()
    {
        var spendRecords = await _spendTrackingRepository.GetAllAsync();
        return spendRecords.Select(MapToDto);
    }

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