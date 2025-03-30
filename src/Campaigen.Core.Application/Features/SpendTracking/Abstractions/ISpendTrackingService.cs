using Campaigen.Core.Application.Features.SpendTracking.DTOs;

namespace Campaigen.Core.Application.Features.SpendTracking.Abstractions;

public interface ISpendTrackingService
{
    Task<SpendRecordDto?> CreateSpendRecordAsync(CreateSpendRecordDto dto);
    Task<SpendRecordDto?> GetSpendRecordAsync(Guid id);
    Task<IEnumerable<SpendRecordDto>> ListSpendRecordsAsync();
    // Add Update and Delete methods later if needed
} 