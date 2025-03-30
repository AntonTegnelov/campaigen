using Campaigen.Core.Application.Features.SpendTracking.Abstractions;
using Campaigen.Core.Domain.Features.SpendTracking;
using Campaigen.Core.Infrastructure.Persistence;

namespace Campaigen.Core.Infrastructure.Features.SpendTracking.Persistence;

public class SpendTrackingRepository : ISpendTrackingRepository
{
    private readonly AppDbContext _context;

    public SpendTrackingRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task AddAsync(SpendRecord spendRecord)
    {
        // EF Core implementation
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid id)
    {
        // EF Core implementation
        throw new NotImplementedException();
    }

    public Task<IEnumerable<SpendRecord>> GetAllAsync()
    {
        // EF Core implementation
        throw new NotImplementedException();
    }

    public Task<SpendRecord?> GetByIdAsync(Guid id)
    {
        // EF Core implementation
        throw new NotImplementedException();
    }

    public Task UpdateAsync(SpendRecord spendRecord)
    {
        // EF Core implementation
        throw new NotImplementedException();
    }
} 