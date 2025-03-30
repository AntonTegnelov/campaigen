using Campaigen.Core.Domain.Features.InfluencerManagement;

namespace Campaigen.Core.Application.Features.InfluencerManagement.Abstractions;

public interface IInfluencerRepository
{
    Task AddAsync(Influencer influencer);
    Task<Influencer?> GetByIdAsync(Guid id);
    Task<IEnumerable<Influencer>> GetAllAsync();
    Task UpdateAsync(Influencer influencer);
    Task DeleteAsync(Guid id);
} 