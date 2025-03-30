using Campaigen.Core.Domain.Features.InfluencerManagement;
using Campaigen.Core.Infrastructure.Features.InfluencerManagement.Persistence;
using Campaigen.Core.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using System.Linq;
using System.Collections.Generic; // Added for List

namespace Campaigen.Infrastructure.Tests;

public class InfluencerRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly InfluencerRepository _repository;

    public InfluencerRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
        _repository = new InfluencerRepository(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task AddAsync_ShouldAddInfluencerToDatabase()
    {
        // Arrange
        var newInfluencer = new Influencer
        {
            Id = Guid.NewGuid(),
            Name = "Test Influencer",
            Handle = "@test",
            Platform = "Insta",
            Niche = "Testing"
        };

        // Act
        await _repository.AddAsync(newInfluencer);

        // Assert
        var retrievedInfluencer = await _context.Influencers.FindAsync(newInfluencer.Id);
        retrievedInfluencer.Should().NotBeNull();
        retrievedInfluencer.Should().BeEquivalentTo(newInfluencer);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCorrectInfluencer_WhenExists()
    {
        // Arrange
        var influencerId = Guid.NewGuid();
        var existingInfluencer = new Influencer
        {
            Id = influencerId,
            Name = "Existing Influencer",
            Handle = "@exists",
            Platform = "TikTok",
            Niche = "Seed"
        };
        _context.Influencers.Add(existingInfluencer);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(influencerId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(existingInfluencer);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllInfluencers()
    {
        // Arrange
        var influencer1 = new Influencer { Id = Guid.NewGuid(), Name = "Inf 1", Handle = "@i1" };
        var influencer2 = new Influencer { Id = Guid.NewGuid(), Name = "Inf 2", Handle = "@i2" };
        _context.Influencers.AddRange(influencer1, influencer2);
        await _context.SaveChangesAsync();

        // Act
        var results = await _repository.GetAllAsync();

        // Assert
        results.Should().NotBeNull();
        results.Should().HaveCount(2);
        results.Should().ContainEquivalentOf(influencer1);
        results.Should().ContainEquivalentOf(influencer2);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmpty_WhenNoInfluencersExist()
    {
        // Arrange

        // Act
        var results = await _repository.GetAllAsync();

        // Assert
        results.Should().NotBeNull();
        results.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyExistingInfluencer()
    {
        // Arrange
        var influencerId = Guid.NewGuid();
        var originalInfluencer = new Influencer
        {
            Id = influencerId,
            Name = "Original Name",
            Handle = "@original",
            Platform = "Platform A",
            Niche = "Niche A"
        };
        _context.Influencers.Add(originalInfluencer);
        await _context.SaveChangesAsync();
        _context.Entry(originalInfluencer).State = EntityState.Detached; // Detach

        var updatedInfluencer = new Influencer
        {
            Id = influencerId, // Same ID
            Name = "Updated Name", // Updated Name
            Handle = "@updated", // Updated Handle
            Platform = originalInfluencer.Platform, // Keep same
            Niche = "Updated Niche" // Updated Niche
        };

        // Act
        await _repository.UpdateAsync(updatedInfluencer);

        // Assert
        var retrievedInfluencer = await _context.Influencers.FindAsync(influencerId);
        retrievedInfluencer.Should().NotBeNull();
        retrievedInfluencer!.Name.Should().Be(updatedInfluencer.Name);
        retrievedInfluencer.Handle.Should().Be(updatedInfluencer.Handle);
        retrievedInfluencer.Platform.Should().Be(originalInfluencer.Platform);
        retrievedInfluencer.Niche.Should().Be(updatedInfluencer.Niche);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveInfluencerFromDatabase()
    {
        // Arrange
        var influencerId = Guid.NewGuid();
        var influencerToDelete = new Influencer { Id = influencerId, Name = "ToDelete" };
        _context.Influencers.Add(influencerToDelete);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(influencerId);

        // Assert
        var retrievedInfluencer = await _context.Influencers.FindAsync(influencerId);
        retrievedInfluencer.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_ShouldDoNothing_WhenInfluencerDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var initialCount = await _context.Influencers.CountAsync();

        // Act
        await _repository.DeleteAsync(nonExistentId);

        // Assert
        var finalCount = await _context.Influencers.CountAsync();
        finalCount.Should().Be(initialCount);
    }
}