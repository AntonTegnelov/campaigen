namespace Campaigen.Core.Application.Features.InfluencerManagement.DTOs;

/// <summary>
/// Data Transfer Object for representing an <see cref="Domain.Features.InfluencerManagement.Influencer"/>.
/// Used typically for returning data from the application layer.
/// </summary>
public class InfluencerDto
{
    /// <summary>
    /// Unique identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Influencer's name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Influencer's handle.
    /// </summary>
    public string? Handle { get; set; }

    /// <summary>
    /// Influencer's primary platform.
    /// </summary>
    public string? Platform { get; set; }

    /// <summary>
    /// Influencer's niche.
    /// </summary>
    public string? Niche { get; set; }
} 