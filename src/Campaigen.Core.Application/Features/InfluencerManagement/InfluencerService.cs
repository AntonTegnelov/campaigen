using Campaigen.Core.Application.Features.InfluencerManagement.Abstractions;
using Campaigen.Core.Application.Features.InfluencerManagement.DTOs;
using Campaigen.Core.Domain.Features.InfluencerManagement;
using System.Linq; // Added for Select

namespace Campaigen.Core.Application.Features.InfluencerManagement;

/// <summary>
/// Service implementation for managing influencer operations.
/// </summary>
public class InfluencerService : IInfluencerService
{
    private readonly IInfluencerRepository _influencerRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="InfluencerService"/> class.
    /// </summary>
    /// <param name="influencerRepository">The influencer repository dependency.</param>
    public InfluencerService(IInfluencerRepository influencerRepository)
    {
        _influencerRepository = influencerRepository;
    }

    /// <inheritdoc />
    public async Task<InfluencerDto?> CreateInfluencerAsync(CreateInfluencerDto dto)
    {
        // Map DTO to domain entity
        var influencer = new Influencer
        {
            Id = Guid.NewGuid(), // Generate ID in the service layer
            Name = dto.Name,
            Handle = dto.Handle,
            Platform = dto.Platform,
            Niche = dto.Niche
        };

        // Persist using the repository
        await _influencerRepository.AddAsync(influencer);

        // Map the created entity back to a DTO to return
        return MapToDto(influencer); // Use helper method
    }

    /// <inheritdoc />
    public async Task<InfluencerDto?> GetInfluencerAsync(Guid id)
    {
        // Retrieve from repository
        var influencer = await _influencerRepository.GetByIdAsync(id);

        // Map to DTO if found, otherwise return null
        return influencer == null ? null : MapToDto(influencer); // Use helper method
    }

    /// <inheritdoc />
    public async Task<IEnumerable<InfluencerDto>> ListInfluencersAsync()
    {
        // Retrieve all from repository
        var influencers = await _influencerRepository.GetAllAsync();

        // Map the list of entities to a list of DTOs using the helper method
        return influencers.Select(MapToDto); // Use helper method, no need for ToList() here
    }

    /// <summary>
    /// Maps an <see cref="Influencer"/> domain entity to an <see cref="InfluencerDto"/>.
    /// </summary>
    /// <param name="influencer">The domain entity to map.</param>
    /// <returns>The resulting DTO.</returns>
    private static InfluencerDto MapToDto(Influencer influencer)
    {
        return new InfluencerDto
        {
            Id = influencer.Id,
            Name = influencer.Name,
            Handle = influencer.Handle,
            Platform = influencer.Platform,
            Niche = influencer.Niche
        };
    }
}