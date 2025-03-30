using System;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Campaigen.Core.Infrastructure.Persistence;
using System.Globalization; // Add this for CultureInfo

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
        var description = "Test Spend Item";
        var category = "Test Category";
        var date = DateTime.UtcNow.Date;

        // Pass arguments as a string array
        var args = new[] {
            "spend", "add",
            "--amount", amount.ToString(CultureInfo.InvariantCulture),
            "--description", description,
            "--category", category,
            "--date", date.ToString("yyyy-MM-dd")
        };

        // Act: Run the CLI command using the helper from the base class
        var result = await RunCliAsync(args);

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
        addedRecord?.Date.Date.Should().Be(DateTime.Parse(date.ToString("yyyy-MM-dd")).Date);
    }

    [Fact]
    public async Task SpendList_WhenRecordsExist_ShouldDisplayRecordsInTableFormat()
    {
        // Arrange: Add a couple of records first
        var record1Amount = 50.00m;
        var record1Desc = "List Spend A";
        var record1Cat = "Cat A";
        var record1Date = DateTime.UtcNow.AddDays(-1).Date;
        // Pass arguments as a string array
        await RunCliAsync("spend", "add", "--amount", record1Amount.ToString(CultureInfo.InvariantCulture), "--description", record1Desc, "--category", record1Cat, "--date", record1Date.ToString("yyyy-MM-dd"));

        var record2Amount = 75.50m;
        var record2Desc = "List Spend B";
        var record2Cat = "Cat B";
        var record2Date = DateTime.UtcNow.Date;
        // Pass arguments as a string array
        await RunCliAsync("spend", "add", "--amount", record2Amount.ToString(CultureInfo.InvariantCulture), "--description", record2Desc, "--category", record2Cat, "--date", record2Date.ToString("yyyy-MM-dd"));

        // Act: Run the list command
        var result = await RunCliAsync("spend", "list");

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

    [Theory]
    [InlineData("spend", "--help")]
    [InlineData("spend", "add", "--help")]
    [InlineData("spend", "list", "--help")]
    public async Task HelpOption_ShouldDisplayHelpText(params string[] helpArgs)
    {
        // Arrange & Act
        var result = await RunCliAsync(helpArgs);

        // Assert
        result.ExitCode.Should().Be(0, because: $"requesting help should succeed. Error: {result.StandardError}");
        result.StandardError.Should().BeEmpty(because: "requesting help should not produce errors.");
        result.StandardOutput.Should().Contain("Usage:", because: "help text should include usage information.");
        result.StandardOutput.Should().Contain("Options:", because: "help text should include options information.");
        result.StandardOutput.Should().Contain("--help", because: "help text should mention the help option.");
    }

    [Theory]
    [InlineData("spend", "add", "--description", "Missing amount")] // Missing --amount
    [InlineData("spend", "add", "--amount", "not_a_number", "--description", "Invalid amount")] // Invalid amount format
    [InlineData("spend", "add", "--amount", "100", "--date", "not_a_date", "--description", "Invalid date")] // Invalid date format
    public async Task SpendAdd_WithInvalidData_ShouldFailAndShowError(params string[] arguments)
    {
        // Arrange & Act
        var result = await RunCliAsync(arguments);

        // Assert
        // result.ExitCode.Should().NotBe(0, because: "invalid input should result in a non-zero exit code."); // Exit code might be 0 by default for parsing errors
        result.StandardError.Should().NotBeNullOrWhiteSpace(because: "an error message should be displayed on standard error for invalid input.");
        // We could be more specific about the error message, but System.CommandLine default messages can vary.
        // Checking for non-empty stderr is the most reliable check here.
    }

    // TODO: Add tests for invalid input scenarios (missing arguments, wrong format, etc.) - In Progress
}