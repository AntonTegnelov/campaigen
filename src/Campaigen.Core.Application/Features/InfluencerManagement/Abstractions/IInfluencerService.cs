using Campaigen.Core.Application.Features.InfluencerManagement.DTOs;

namespace Campaigen.Core.Application.Features.InfluencerManagement.Abstractions;

/// <summary>
/// Interface defining the application service operations for influencer management.
/// Mediates between the presentation layer (e.g., CLI) and the data access layer (repository).
/// Adheres to the Interface Segregation Principle (ISP).
/// </summary>
public interface IInfluencerService
{
    /// <summary>
    /// Creates a new influencer based on the provided data.
    /// </summary>
    /// <param name="dto">Data Transfer Object containing the details for the new influencer.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains an <see cref="InfluencerDto"/> of the created influencer, or null if creation failed.</returns>
    Task<InfluencerDto?> CreateInfluencerAsync(CreateInfluencerDto dto);

    /// <summary>
    /// Retrieves a specific influencer by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the influencer.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the <see cref="InfluencerDto"/> if found, otherwise null.</returns>
    Task<InfluencerDto?> GetInfluencerAsync(Guid id);

    /// <summary>
    /// Retrieves a list of all influencers.
    /// </summary>
    /// <returns>A task representing the asynchronous operation. The task result contains an enumerable collection of <see cref="InfluencerDto"/>.</returns>
    Task<IEnumerable<InfluencerDto>> ListInfluencersAsync();

    // TODO: Define Update and Delete service methods if business logic requires them.
} 