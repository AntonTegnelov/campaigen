namespace Campaigen.Core.Application.Features.InfluencerManagement.DTOs;

public class CreateInfluencerDto
{
    public string? Name { get; set; }
    public string? Handle { get; set; }
    public string? Platform { get; set; }
    public string? Niche { get; set; }
    // Add validation attributes later if needed
} 