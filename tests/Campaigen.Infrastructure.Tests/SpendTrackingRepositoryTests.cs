using Campaigen.Core.Domain.Features.SpendTracking;
using Campaigen.Core.Infrastructure.Features.SpendTracking.Persistence;
using Campaigen.Core.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using System.Linq;

namespace Campaigen.Infrastructure.Tests;

public class SpendTrackingRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly SpendTrackingRepository _repository;

    public SpendTrackingRepositoryTests()
    {
        // Setup InMemory database with a unique name per test run to ensure isolation
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
        _repository = new SpendTrackingRepository(_context);
    }

    // Dispose the context after each test to clean up the InMemory database
    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
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
        // AddAsync calls SaveChangesAsync internally

        // Assert
        var retrievedRecord = await _context.SpendRecords.FindAsync(newRecord.Id);
        retrievedRecord.Should().NotBeNull();
        retrievedRecord.Should().BeEquivalentTo(newRecord);
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
        await _context.SaveChangesAsync(); // Seed data directly to context for this test

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

    [Fact]
    public async Task UpdateAsync_ShouldModifyExistingSpendRecord()
    {
        // Arrange: Seed an initial record
        var recordId = Guid.NewGuid();
        var originalRecord = new SpendRecord
        {
            Id = recordId,
            Date = DateTime.UtcNow.AddDays(-2),
            Amount = 200m,
            Description = "Original Description",
            Category = "Initial"
        };
        _context.SpendRecords.Add(originalRecord);
        await _context.SaveChangesAsync();
        // Detach the seeded record so EF Core doesn't track the original instance
        _context.Entry(originalRecord).State = EntityState.Detached;

        // Arrange: Create an updated version with the same ID
        var updatedRecord = new SpendRecord
        {
            Id = recordId, // Same ID
            Date = originalRecord.Date, // Keep date same
            Amount = 250.75m, // Updated Amount
            Description = "Updated Description", // Updated Description
            Category = "Updated" // Updated Category
        };

        // Act: Call the repository's UpdateAsync
        await _repository.UpdateAsync(updatedRecord);

        // Assert: Retrieve the record from the context and check updated fields
        var retrievedRecord = await _context.SpendRecords.FindAsync(recordId);
        retrievedRecord.Should().NotBeNull();
        retrievedRecord!.Amount.Should().Be(updatedRecord.Amount);
        retrievedRecord.Description.Should().Be(updatedRecord.Description);
        retrievedRecord.Category.Should().Be(updatedRecord.Category);
        retrievedRecord.Date.Should().Be(originalRecord.Date); // Verify date didn't change
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveSpendRecordFromDatabase()
    {
        // Arrange: Seed a record to delete
        var recordId = Guid.NewGuid();
        var recordToDelete = new SpendRecord { Id = recordId, Amount = 50m, Date = DateTime.UtcNow };
        _context.SpendRecords.Add(recordToDelete);
        await _context.SaveChangesAsync();

        // Act: Call repository's DeleteAsync
        await _repository.DeleteAsync(recordId);

        // Assert: Verify the record is no longer found in the context
        var retrievedRecord = await _context.SpendRecords.FindAsync(recordId);
        retrievedRecord.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_ShouldDoNothing_WhenRecordDoesNotExist()
    {
        // Arrange: Ensure no records exist or get initial count
        var nonExistentId = Guid.NewGuid();
        var initialCount = await _context.SpendRecords.CountAsync();

        // Act: Call DeleteAsync with a non-existent ID
        await _repository.DeleteAsync(nonExistentId);

        // Assert: Verify the count hasn't changed
        var finalCount = await _context.SpendRecords.CountAsync();
        finalCount.Should().Be(initialCount);
    }
}
