namespace Campaigen.Core.Application.Features.InfluencerManagement.DTOs;

/// <summary>
/// Data Transfer Object for creating a new <see cref="Domain.Features.InfluencerManagement.Influencer"/>.
/// Used typically for receiving data in the application layer.
/// </summary>
public class CreateInfluencerDto
{
    /// <summary>
    /// Influencer's name. This is required.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Optional influencer handle.
    /// </summary>
    public string? Handle { get; set; }

    /// <summary>
    /// Optional primary platform.
    /// </summary>
    public string? Platform { get; set; }

    /// <summary>
    /// Optional niche.
    /// </summary>
    public string? Niche { get; set; }

    // TODO: Consider adding validation attributes (e.g., [Required]) using System.ComponentModel.DataAnnotations
} 