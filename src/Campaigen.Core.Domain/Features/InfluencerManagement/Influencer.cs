namespace Campaigen.Core.Domain.Features.InfluencerManagement;

public class Influencer
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Handle { get; set; }
    public string? Platform { get; set; }
    public string? Niche { get; set; }
    // Add other relevant properties as needed
} 