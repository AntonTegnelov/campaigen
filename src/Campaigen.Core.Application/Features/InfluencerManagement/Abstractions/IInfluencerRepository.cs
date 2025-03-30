using Campaigen.Core.Domain.Features.InfluencerManagement;

namespace Campaigen.Core.Application.Features.InfluencerManagement.Abstractions;

/// <summary>
/// Interface defining the repository operations for <see cref="Influencer"/> entities.
/// Adheres to the Interface Segregation Principle (ISP).
/// </summary>
public interface IInfluencerRepository
{
    /// <summary>
    /// Adds a new influencer to the data store.
    /// </summary>
    /// <param name="influencer">The influencer entity to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(Influencer influencer);

    /// <summary>
    /// Retrieves an influencer by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the influencer.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the <see cref="Influencer"/> if found, otherwise null.</returns>
    Task<Influencer?> GetByIdAsync(Guid id);

    /// <summary>
    /// Retrieves all influencers from the data store.
    /// </summary>
    /// <returns>A task representing the asynchronous operation. The task result contains an enumerable collection of <see cref="Influencer"/>.</returns>
    Task<IEnumerable<Influencer>> GetAllAsync();

    /// <summary>
    /// Updates an existing influencer in the data store.
    /// </summary>
    /// <param name="influencer">The influencer entity with updated values.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(Influencer influencer);

    /// <summary>
    /// Deletes an influencer from the data store by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the influencer to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(Guid id);
} 