using Moq;
using FluentAssertions;
using Campaigen.Core.Application.Features.InfluencerManagement;
using Campaigen.Core.Application.Features.InfluencerManagement.Abstractions;
using Campaigen.Core.Application.Features.InfluencerManagement.DTOs;
using Campaigen.Core.Domain.Features.InfluencerManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Campaigen.Application.Tests.Features.InfluencerManagement;

public class InfluencerServiceTests
{
    private readonly Mock<IInfluencerRepository> _mockRepo;
    private readonly InfluencerService _service;

    public InfluencerServiceTests()
    {
        _mockRepo = new Mock<IInfluencerRepository>();
        _service = new InfluencerService(_mockRepo.Object);
    }

    [Fact]
    public async Task CreateInfluencerAsync_ShouldCallRepositoryAddAndReturnDto()
    {
        // Arrange
        var createDto = new CreateInfluencerDto { Name = "Test Influencer", Handle = "@test", Platform = "Insta", Niche = "Tech" };
        Influencer? capturedInfluencer = null;

        // Capture the influencer passed to AddAsync
        _mockRepo.Setup(repo => repo.AddAsync(It.IsAny<Influencer>()))
                 .Callback<Influencer>(inf => capturedInfluencer = inf)
                 .Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateInfluencerAsync(createDto);

        // Assert
        _mockRepo.Verify(repo => repo.AddAsync(It.IsAny<Influencer>()), Times.Once);
        result.Should().NotBeNull();
        result.Should().BeOfType<InfluencerDto>();
        result!.Name.Should().Be(createDto.Name);
        result!.Handle.Should().Be(createDto.Handle);
        result!.Platform.Should().Be(createDto.Platform);
        result!.Niche.Should().Be(createDto.Niche);
        result!.Id.Should().NotBeEmpty(); // Check that an ID was generated

        // Verify captured entity matches DTO (except ID)
        capturedInfluencer.Should().NotBeNull();
        capturedInfluencer!.Name.Should().Be(createDto.Name);
        capturedInfluencer!.Handle.Should().Be(createDto.Handle);
        capturedInfluencer!.Platform.Should().Be(createDto.Platform);
        capturedInfluencer!.Niche.Should().Be(createDto.Niche);
        capturedInfluencer!.Id.Should().Be(result.Id); // Ensure the ID passed to repo matches returned DTO
    }

    [Fact]
    public async Task GetInfluencerAsync_ShouldReturnDto_WhenInfluencerExists()
    {
        // Arrange
        var influencerId = Guid.NewGuid();
        var influencer = new Influencer { Id = influencerId, Name = "Found Influencer", Handle = "@found", Platform = "TikTok", Niche = "Lifestyle" };
        _mockRepo.Setup(repo => repo.GetByIdAsync(influencerId)).ReturnsAsync(influencer);

        // Act
        var result = await _service.GetInfluencerAsync(influencerId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<InfluencerDto>();
        result!.Id.Should().Be(influencerId);
        result!.Name.Should().Be(influencer.Name);
        result!.Handle.Should().Be(influencer.Handle);
        result!.Platform.Should().Be(influencer.Platform);
        result!.Niche.Should().Be(influencer.Niche);
    }

    [Fact]
    public async Task GetInfluencerAsync_ShouldReturnNull_WhenInfluencerDoesNotExist()
    {
        // Arrange
        var influencerId = Guid.NewGuid();
        _mockRepo.Setup(repo => repo.GetByIdAsync(influencerId)).ReturnsAsync((Influencer?)null);

        // Act
        var result = await _service.GetInfluencerAsync(influencerId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task ListInfluencersAsync_ShouldReturnMappedDtos_WhenInfluencersExist()
    {
        // Arrange
        var influencers = new List<Influencer>
        {
            new Influencer { Id = Guid.NewGuid(), Name = "Influencer 1", Handle = "@inf1", Platform = "YouTube", Niche = "Gaming" },
            new Influencer { Id = Guid.NewGuid(), Name = "Influencer 2", Handle = "@inf2", Platform = "Insta", Niche = "Beauty" }
        };
        _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(influencers);

        // Act
        var result = await _service.ListInfluencersAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().ContainItemsAssignableTo<InfluencerDto>();

        var resultList = result.ToList();
        resultList[0].Id.Should().Be(influencers[0].Id);
        resultList[0].Name.Should().Be(influencers[0].Name);
        resultList[1].Id.Should().Be(influencers[1].Id);
        resultList[1].Name.Should().Be(influencers[1].Name);
    }

    [Fact]
    public async Task ListInfluencersAsync_ShouldReturnEmptyList_WhenNoInfluencersExist()
    {
        // Arrange
        var emptyList = new List<Influencer>();
        _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(emptyList);

        // Act
        var result = await _service.ListInfluencersAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}