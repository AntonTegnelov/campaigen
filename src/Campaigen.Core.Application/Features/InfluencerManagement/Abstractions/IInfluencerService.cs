using Campaigen.Core.Application.Features.InfluencerManagement.DTOs;

namespace Campaigen.Core.Application.Features.InfluencerManagement.Abstractions;

public interface IInfluencerService
{
    Task<InfluencerDto?> CreateInfluencerAsync(CreateInfluencerDto dto);
    Task<InfluencerDto?> GetInfluencerAsync(Guid id);
    Task<IEnumerable<InfluencerDto>> ListInfluencersAsync();
    // Add Update/Delete methods if needed
} 