using Campaigen.Core.Domain.Features.SpendTracking;

namespace Campaigen.Core.Application.Features.SpendTracking.Abstractions;

public interface ISpendTrackingRepository
{
    Task AddAsync(SpendRecord spendRecord);
    Task<SpendRecord?> GetByIdAsync(Guid id);
    Task<IEnumerable<SpendRecord>> GetAllAsync();
    Task UpdateAsync(SpendRecord spendRecord);
    Task DeleteAsync(Guid id);
} 