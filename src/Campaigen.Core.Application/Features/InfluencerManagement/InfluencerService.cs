using Campaigen.Core.Application.Features.InfluencerManagement.Abstractions;
using Campaigen.Core.Application.Features.InfluencerManagement.DTOs;

namespace Campaigen.Core.Application.Features.InfluencerManagement;

public class InfluencerService : IInfluencerService
{
    private readonly IInfluencerRepository _influencerRepository;

    public InfluencerService(IInfluencerRepository influencerRepository)
    {
        _influencerRepository = influencerRepository;
    }

    public Task<InfluencerDto?> CreateInfluencerAsync(CreateInfluencerDto dto)
    {
        // Implementation using repository and mapping
        throw new NotImplementedException();
    }

    public Task<InfluencerDto?> GetInfluencerAsync(Guid id)
    {
        // Implementation using repository and mapping
        throw new NotImplementedException();
    }

    public Task<IEnumerable<InfluencerDto>> ListInfluencersAsync()
    {
        // Implementation using repository and mapping
        throw new NotImplementedException();
    }
} 