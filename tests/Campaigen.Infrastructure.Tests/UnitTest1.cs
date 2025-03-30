using Campaigen.Core.Domain.Features.SpendTracking;
using Campaigen.Core.Infrastructure.Features.SpendTracking.Persistence;
using Campaigen.Core.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions; // Added for assertions
using System.Linq; // Added for Linq methods

namespace Campaigen.Infrastructure.Tests;

// Renamed from UnitTest1
public class SpendTrackingRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly SpendTrackingRepository _repository;

    public SpendTrackingRepositoryTests()
    {
        // Setup InMemory database
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique name per test run
            .Options;
        _context = new AppDbContext(options);
        _repository = new SpendTrackingRepository(_context);
    }

    public void Dispose()
    {
        // Clean up the context after each test
        _context.Dispose();
        GC.SuppressFinalize(this); // Recommended practice for IDisposable
    }

    [Fact]
    public async Task AddAsync_ShouldAddSpendRecordToDatabase()
    {
        // Arrange
        var newRecord = new SpendRecord
        {
            Id = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            Amount = 100.50m,
            Description = "Test Spend",
            Category = "Testing"
        };

        // Act
        await _repository.AddAsync(newRecord);
        // No need to call SaveChangesAsync here, AddAsync in repository does it

        // Assert
        var retrievedRecord = await _context.SpendRecords.FindAsync(newRecord.Id);
        retrievedRecord.Should().NotBeNull();
        retrievedRecord.Should().BeEquivalentTo(newRecord); // Compare property values
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCorrectSpendRecord_WhenExists()
    {
        // Arrange
        var recordId = Guid.NewGuid();
        var existingRecord = new SpendRecord
        {
            Id = recordId,
            Date = DateTime.UtcNow.AddDays(-1),
            Amount = 55.00m,
            Description = "Existing Spend",
            Category = "Seed Data"
        };
        _context.SpendRecords.Add(existingRecord);
        await _context.SaveChangesAsync(); // Seed data

        // Act
        var result = await _repository.GetByIdAsync(recordId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(existingRecord);
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
    public async Task GetAllAsync_ShouldReturnAllSpendRecords()
    {
        // Arrange
        var record1 = new SpendRecord { Id = Guid.NewGuid(), Date = DateTime.UtcNow, Amount = 10m, Description = "Rec 1" };
        var record2 = new SpendRecord { Id = Guid.NewGuid(), Date = DateTime.UtcNow, Amount = 20m, Description = "Rec 2" };
        _context.SpendRecords.AddRange(record1, record2);
        await _context.SaveChangesAsync();

        // Act
        var results = await _repository.GetAllAsync();

        // Assert
        results.Should().NotBeNull();
        results.Should().HaveCount(2);
        results.Should().ContainEquivalentOf(record1);
        results.Should().ContainEquivalentOf(record2);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmpty_WhenNoRecordsExist()
    {
        // Arrange - No records added

        // Act
        var results = await _repository.GetAllAsync();

        // Assert
        results.Should().NotBeNull();
        results.Should().BeEmpty();
    }

     // TODO: Add tests for UpdateAsync and DeleteAsync
}