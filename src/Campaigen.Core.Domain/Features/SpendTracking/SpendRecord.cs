namespace Campaigen.Core.Domain.Features.SpendTracking;

/// <summary>
/// Represents a single record of marketing spend.
/// </summary>
public class SpendRecord
{
    /// <summary>
    /// Unique identifier for the spend record.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The date when the spend occurred.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// The monetary amount spent.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Optional description of the spend.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Optional category for the spend (e.g., Advertising, Software, Travel).
    /// </summary>
    public string? Category { get; set; }
} 