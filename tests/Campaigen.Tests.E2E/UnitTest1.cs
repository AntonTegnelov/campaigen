using System;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Campaigen.Core.Infrastructure.Persistence;

namespace Campaigen.Tests.E2E;

/// <summary>
/// E2E tests for the 'spend' commands in the CLI.
/// </summary>
public class SpendCommandsTests : E2ETestBase // Inherit from the base class
{
    [Fact]
    public async Task SpendAdd_WithValidData_ShouldSucceedAndAddRecord()
    {
        // Arrange
        var amount = 123.45m;
        var description = "Test spend item";
        var category = "Test Category";
        var date = DateTime.UtcNow.ToString("yyyy-MM-dd"); // Use consistent format

        var arguments = $"spend add --amount {amount} --description \"{description}\" --category \"{category}\" --date {date}";

        // Act: Run the CLI command using the helper from the base class
        var result = await RunCliAsync(arguments);

        // Assert - CLI Output
        result.ExitCode.Should().Be(0, because: $"the command should execute successfully. Error: {result.StandardError}");
        result.StandardOutput.Should().Contain("Spend record added successfully.", because: "the user should receive success feedback.");
        result.StandardError.Should().BeEmpty(because: "no errors should occur during successful addition.");

        // Assert - Database Verification (Optional but recommended)
        // Create a new DbContext instance pointing to the SAME test database
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlite(TestConnectionString); // Use the connection string from the base class
        using var verifyContext = new AppDbContext(optionsBuilder.Options);

        var addedRecord = await verifyContext.SpendRecords.FirstOrDefaultAsync(r => r.Description == description);

        addedRecord.Should().NotBeNull(because: "the record should have been added to the database.");
        addedRecord?.Amount.Should().Be(amount);
        addedRecord?.Category.Should().Be(category);
        // Use DateOnly comparison for date part if needed, or check for close enough date/time
        addedRecord?.Date.Date.Should().Be(DateTime.Parse(date).Date);
    }

    // TODO: Add test for 'spend list'
    // TODO: Add tests for invalid input scenarios (missing arguments, wrong format, etc.)
}