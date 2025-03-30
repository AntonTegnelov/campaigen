namespace Campaigen.Core.Domain.Features.InfluencerManagement;

/// <summary>
/// Represents a marketing influencer.
/// </summary>
public class Influencer
{
    /// <summary>
    /// Unique identifier for the influencer.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The name of the influencer.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// The influencer's primary social media handle (e.g., @username).
    /// </summary>
    public string? Handle { get; set; }

    /// <summary>
    /// The primary platform the influencer uses (e.g., Instagram, TikTok, YouTube).
    /// </summary>
    public string? Platform { get; set; }

    /// <summary>
    /// The influencer's content niche or category (e.g., Tech, Beauty, Gaming).
    /// </summary>
    public string? Niche { get; set; }

    // TODO: Consider adding more properties like contact info, engagement rates, etc.
} 