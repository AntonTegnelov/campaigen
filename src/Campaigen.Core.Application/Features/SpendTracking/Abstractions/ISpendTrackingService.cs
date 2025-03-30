using Campaigen.Core.Application.Features.SpendTracking.DTOs;

namespace Campaigen.Core.Application.Features.SpendTracking.Abstractions;

/// <summary>
/// Interface defining the application service operations for spend tracking.
/// Mediates between the presentation layer (e.g., CLI) and the data access layer (repository).
/// Adheres to the Interface Segregation Principle (ISP).
/// </summary>
public interface ISpendTrackingService
{
    /// <summary>
    /// Creates a new spend record based on the provided data.
    /// </summary>
    /// <param name="dto">Data Transfer Object containing the details for the new spend record.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a <see cref="SpendRecordDto"/> of the created record, or null if creation failed.</returns>
    Task<SpendRecordDto?> CreateSpendRecordAsync(CreateSpendRecordDto dto);

    /// <summary>
    /// Retrieves a specific spend record by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the spend record.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the <see cref="SpendRecordDto"/> if found, otherwise null.</returns>
    Task<SpendRecordDto?> GetSpendRecordAsync(Guid id);

    /// <summary>
    /// Retrieves a list of all spend records.
    /// </summary>
    /// <returns>A task representing the asynchronous operation. The task result contains an enumerable collection of <see cref="SpendRecordDto"/>.</returns>
    Task<IEnumerable<SpendRecordDto>> ListSpendRecordsAsync();

    // TODO: Define Update and Delete service methods if business logic requires them
    // (e.g., validation, specific mapping, event publishing before/after persistence).
}