using Campaigen.Core.Application.Features.InfluencerManagement.Abstractions;
using Campaigen.Core.Application.Features.InfluencerManagement.DTOs;
using Campaigen.Core.Domain.Features.InfluencerManagement;

namespace Campaigen.Core.Application.Features.InfluencerManagement;

public class InfluencerService : IInfluencerService
{
    private readonly IInfluencerRepository _influencerRepository;

    public InfluencerService(IInfluencerRepository influencerRepository)
    {
        _influencerRepository = influencerRepository;
    }

    public async Task<InfluencerDto?> CreateInfluencerAsync(CreateInfluencerDto dto)
    {
        // Implementation using repository and mapping
        var influencer = new Influencer
        {
            Id = Guid.NewGuid(), // Generate ID in the service layer
            Name = dto.Name,
            Handle = dto.Handle,
            Platform = dto.Platform,
            Niche = dto.Niche
        };

        await _influencerRepository.AddAsync(influencer);

        // Map the created entity back to a DTO to return
        return new InfluencerDto
        {
            Id = influencer.Id,
            Name = influencer.Name,
            Handle = influencer.Handle,
            Platform = influencer.Platform,
            Niche = influencer.Niche
        };
    }

    public async Task<InfluencerDto?> GetInfluencerAsync(Guid id)
    {
        // Implementation using repository and mapping
        var influencer = await _influencerRepository.GetByIdAsync(id);

        if (influencer == null)
        {
            return null; // Return null if not found
        }

        // Map the entity to a DTO
        return new InfluencerDto
        {
            Id = influencer.Id,
            Name = influencer.Name,
            Handle = influencer.Handle,
            Platform = influencer.Platform,
            Niche = influencer.Niche
        };
    }

    public async Task<IEnumerable<InfluencerDto>> ListInfluencersAsync()
    {
        // Implementation using repository and mapping
        var influencers = await _influencerRepository.GetAllAsync();

        // Map the list of entities to a list of DTOs
        return influencers.Select(influencer => new InfluencerDto
        {
            Id = influencer.Id,
            Name = influencer.Name,
            Handle = influencer.Handle,
            Platform = influencer.Platform,
            Niche = influencer.Niche
        }).ToList(); // Materialize the list here
    }
} 