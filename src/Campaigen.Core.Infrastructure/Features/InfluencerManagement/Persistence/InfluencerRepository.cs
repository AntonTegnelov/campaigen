using Campaigen.Core.Application.Features.InfluencerManagement.Abstractions;
using Campaigen.Core.Domain.Features.InfluencerManagement;
using Campaigen.Core.Infrastructure.Persistence;

namespace Campaigen.Core.Infrastructure.Features.InfluencerManagement.Persistence;

public class InfluencerRepository : IInfluencerRepository
{
    private readonly AppDbContext _context;

    public InfluencerRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task AddAsync(Influencer influencer)
    {
        // EF Core implementation
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid id)
    {
        // EF Core implementation
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Influencer>> GetAllAsync()
    {
        // EF Core implementation
        throw new NotImplementedException();
    }

    public Task<Influencer?> GetByIdAsync(Guid id)
    {
        // EF Core implementation
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Influencer influencer)
    {
        // EF Core implementation
        throw new NotImplementedException();
    }
} 