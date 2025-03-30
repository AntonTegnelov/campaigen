namespace Campaigen.Core.Application.Features.SpendTracking.DTOs;

/// <summary>
/// Data Transfer Object for representing a <see cref="Domain.Features.SpendTracking.SpendRecord"/>.
/// Used typically for returning data from the application layer.
/// </summary>
public class SpendRecordDto
{
    /// <summary>
    /// Unique identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Date of the spend.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Amount spent.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Optional description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Optional category.
    /// </summary>
    public string? Category { get; set; }
} 