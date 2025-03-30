using Moq;
using FluentAssertions;
using Campaigen.Core.Application.Features.SpendTracking;
using Campaigen.Core.Application.Features.SpendTracking.Abstractions;
using Campaigen.Core.Application.Features.SpendTracking.DTOs;
using Campaigen.Core.Domain.Features.SpendTracking;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Campaigen.Application.Tests.Features.SpendTracking;

public class SpendTrackingServiceTests
{
    private readonly Mock<ISpendTrackingRepository> _mockRepo;
    private readonly SpendTrackingService _service;

    public SpendTrackingServiceTests()
    {
        _mockRepo = new Mock<ISpendTrackingRepository>();
        _service = new SpendTrackingService(_mockRepo.Object);
    }

    [Fact]
    public async Task ListSpendRecordsAsync_ShouldReturnMappedDtos_WhenRecordsExist()
    {
        // Arrange
        var records = new List<SpendRecord>
        {
            new SpendRecord { Id = Guid.NewGuid(), Amount = 100, Date = DateTime.UtcNow, Description = "Test 1" },
            new SpendRecord { Id = Guid.NewGuid(), Amount = 200, Date = DateTime.UtcNow.AddDays(-1), Description = "Test 2" }
        };
        _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(records);

        // Act
        var result = await _service.ListSpendRecordsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.First().Id.Should().Be(records.First().Id);
        result.First().Amount.Should().Be(records.First().Amount);
        result.Last().Description.Should().Be(records.Last().Description);
    }

    [Fact]
    public async Task ListSpendRecordsAsync_ShouldReturnEmptyList_WhenNoRecordsExist()
    {
        // Arrange
        _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<SpendRecord>());

        // Act
        var result = await _service.ListSpendRecordsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetSpendRecordAsync_ShouldReturnMappedDto_WhenRecordExists()
    {
        // Arrange
        var recordId = Guid.NewGuid();
        var record = new SpendRecord { Id = recordId, Amount = 150, Date = DateTime.UtcNow };
        _mockRepo.Setup(repo => repo.GetByIdAsync(recordId)).ReturnsAsync(record);

        // Act
        var result = await _service.GetSpendRecordAsync(recordId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(recordId);
        result.Amount.Should().Be(record.Amount);
    }

    [Fact]
    public async Task GetSpendRecordAsync_ShouldReturnNull_WhenRecordDoesNotExist()
    {
        // Arrange
        var recordId = Guid.NewGuid();
        _mockRepo.Setup(repo => repo.GetByIdAsync(recordId)).ReturnsAsync((SpendRecord?)null);

        // Act
        var result = await _service.GetSpendRecordAsync(recordId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateSpendRecordAsync_ShouldCallAddAsyncAndReturnMappedDto()
    {
        // Arrange
        var createDto = new CreateSpendRecordDto
        {
            Amount = 250,
            Date = DateTime.UtcNow,
            Description = "New Spend",
            Category = "Test Category"
        };
        _mockRepo.Setup(repo => repo.AddAsync(It.IsAny<SpendRecord>())).Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateSpendRecordAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result!.Amount.Should().Be(createDto.Amount);
        result.Description.Should().Be(createDto.Description);
        result.Category.Should().Be(createDto.Category);
        _mockRepo.Verify(repo => repo.AddAsync(It.Is<SpendRecord>(r =>
            r.Amount == createDto.Amount &&
            r.Description == createDto.Description &&
            r.Category == createDto.Category)), Times.Once);
    }
}