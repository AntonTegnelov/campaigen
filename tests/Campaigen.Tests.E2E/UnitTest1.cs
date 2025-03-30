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

    [Fact]
    public async Task SpendList_WhenRecordsExist_ShouldDisplayRecordsInTableFormat()
    {
        // Arrange: Add a couple of records first
        var record1Date = DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-dd");
        var record1Desc = "List Test Item 1";
        var record1Cat = "Category A";
        var record1Amount = 50.75m;
        await RunCliAsync($"spend add --amount {record1Amount} --description \"{record1Desc}\" --category \"{record1Cat}\" --date {record1Date}");

        var record2Date = DateTime.UtcNow.ToString("yyyy-MM-dd");
        var record2Desc = "List Test Item 2";
        var record2Cat = "Category B";
        var record2Amount = 199.99m;
        await RunCliAsync($"spend add --amount {record2Amount} --description \"{record2Desc}\" --category \"{record2Cat}\" --date {record2Date}");

        // Act: Run the list command
        var result = await RunCliAsync("spend list");

        // Assert
        result.ExitCode.Should().Be(0, because: $"'spend list' should execute successfully. Error: {result.StandardError}");
        result.StandardError.Should().BeEmpty(because: "no errors should occur when listing.");

        // Verify headers and the presence of both records in the output
        result.StandardOutput.Should().Contain("ID", because: "the output table should have an ID header.");
        result.StandardOutput.Should().Contain("Date", because: "the output table should have a Date header.");
        result.StandardOutput.Should().Contain("Amount", because: "the output table should have an Amount header.");
        result.StandardOutput.Should().Contain("Category", because: "the output table should have a Category header.");
        result.StandardOutput.Should().Contain("Description", because: "the output table should have a Description header.");

        // Check for specific record details (adjust formatting checks as needed based on actual output)
        result.StandardOutput.Should().Contain(record1Desc, because: "the first record's description should be listed.");
        result.StandardOutput.Should().Contain(record1Cat, because: "the first record's category should be listed.");
        result.StandardOutput.Should().Contain(record1Amount.ToString("F2"), because: "the first record's amount should be listed formatted to 2 decimal places."); // Check formatted amount

        result.StandardOutput.Should().Contain(record2Desc, because: "the second record's description should be listed.");
        result.StandardOutput.Should().Contain(record2Cat, because: "the second record's category should be listed.");
        result.StandardOutput.Should().Contain(record2Amount.ToString("F2"), because: "the second record's amount should be listed formatted to 2 decimal places."); // Check formatted amount

    }

    // TODO: Add test for 'spend list'
    // TODO: Add tests for invalid input scenarios (missing arguments, wrong format, etc.)
}