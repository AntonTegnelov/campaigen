namespace Campaigen.Core.Application.Features.SpendTracking.DTOs;

/// <summary>
/// Data Transfer Object for creating a new <see cref="Domain.Features.SpendTracking.SpendRecord"/>.
/// Used typically for receiving data in the application layer.
/// </summary>
public class CreateSpendRecordDto
{
    /// <summary>
    /// Date of the spend. Defaults to current UTC time if not provided by the caller.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Amount spent. This is required.
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

    // TODO: Consider adding validation attributes (e.g., [Required], [Range]) using System.ComponentModel.DataAnnotations
}