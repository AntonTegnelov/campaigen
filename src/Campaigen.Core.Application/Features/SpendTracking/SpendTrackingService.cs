using Campaigen.Core.Application.Features.SpendTracking.Abstractions;
using Campaigen.Core.Application.Features.SpendTracking.DTOs;

namespace Campaigen.Core.Application.Features.SpendTracking;

public class SpendTrackingService : ISpendTrackingService
{
    private readonly ISpendTrackingRepository _spendTrackingRepository;

    public SpendTrackingService(ISpendTrackingRepository spendTrackingRepository)
    {
        _spendTrackingRepository = spendTrackingRepository;
    }

    public Task<SpendRecordDto?> CreateSpendRecordAsync(CreateSpendRecordDto dto)
    {
        // Implementation using repository and mapping
        throw new NotImplementedException();
    }

    public Task<SpendRecordDto?> GetSpendRecordAsync(Guid id)
    {
        // Implementation using repository and mapping
        throw new NotImplementedException();
    }

    public Task<IEnumerable<SpendRecordDto>> ListSpendRecordsAsync()
    {
        // Implementation using repository and mapping
        throw new NotImplementedException();
    }
} 