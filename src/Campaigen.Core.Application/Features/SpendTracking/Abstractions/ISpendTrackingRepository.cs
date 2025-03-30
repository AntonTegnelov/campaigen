using Campaigen.Core.Domain.Features.SpendTracking;

namespace Campaigen.Core.Application.Features.SpendTracking.Abstractions;

/// <summary>
/// Interface defining the repository operations for <see cref="SpendRecord"/> entities.
/// Adheres to the Interface Segregation Principle (ISP).
/// </summary>
public interface ISpendTrackingRepository
{
    /// <summary>
    /// Adds a new spend record to the data store.
    /// </summary>
    /// <param name="spendRecord">The spend record entity to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(SpendRecord spendRecord);

    /// <summary>
    /// Retrieves a spend record by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the spend record.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the <see cref="SpendRecord"/> if found, otherwise null.</returns>
    Task<SpendRecord?> GetByIdAsync(Guid id);

    /// <summary>
    /// Retrieves all spend records from the data store.
    /// </summary>
    /// <returns>A task representing the asynchronous operation. The task result contains an enumerable collection of <see cref="SpendRecord"/>.</returns>
    Task<IEnumerable<SpendRecord>> GetAllAsync();

    /// <summary>
    /// Updates an existing spend record in the data store.
    /// </summary>
    /// <param name="spendRecord">The spend record entity with updated values.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(SpendRecord spendRecord);

    /// <summary>
    /// Deletes a spend record from the data store by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the spend record to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(Guid id);
}