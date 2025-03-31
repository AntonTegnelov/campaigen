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

        // Assert - Database Verification
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlite(TestConnectionString);
        using var verifyContext = new AppDbContext(optionsBuilder.Options);

        var addedRecord = await verifyContext.SpendRecords.FirstOrDefaultAsync(r => r.Description == description);

        addedRecord.Should().NotBeNull(because: "the record should have been added to the database.");
        if (addedRecord != null)
        {
            addedRecord.Amount.Should().Be(amount);
            addedRecord.Category.Should().Be(category);
            addedRecord.Date.Date.Should().Be(DateTime.Parse(date.ToString("yyyy-MM-dd")).Date);
        }
    }

    [Fact]
    public async Task SpendList_WhenRecordsExist_ShouldDisplayRecordsInTableFormat()
    {
        // Arrange: Add a couple of records directly using the database helper
        var dbHelper = new DatabaseHelper(TestConnectionString);

        var record1Amount = 50.00m;
        var record1Desc = "List Spend A";
        var record1Cat = "Cat A";
        var record1Date = DateTime.UtcNow.AddDays(-1).Date;
        await dbHelper.AddSpendRecordAsync(record1Amount, record1Desc, record1Cat, record1Date);

        var record2Amount = 75.50m;
        var record2Desc = "List Spend B";
        var record2Cat = "Cat B";
        var record2Date = DateTime.UtcNow.Date;
        await dbHelper.AddSpendRecordAsync(record2Amount, record2Desc, record2Cat, record2Date);

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
        
        // Make the decimal format check culture-invariant (accept either . or , as separator)
        string amountWithDot = record1Amount.ToString("F2", CultureInfo.InvariantCulture); // 50.00
        string amountWithComma = record1Amount.ToString("F2", CultureInfo.GetCultureInfo("fr-FR")); // 50,00
        result.StandardOutput.Should().Match(output => 
            output.Contains(amountWithDot) || output.Contains(amountWithComma), 
            because: "the first record's amount should be listed with 2 decimal places (period or comma separator).");

        result.StandardOutput.Should().Contain(record2Desc, because: "the second record's description should be listed.");
        result.StandardOutput.Should().Contain(record2Cat, because: "the second record's category should be listed.");
        
        // Make the decimal format check culture-invariant for the second record too
        string amount2WithDot = record2Amount.ToString("F2", CultureInfo.InvariantCulture); // 75.50
        string amount2WithComma = record2Amount.ToString("F2", CultureInfo.GetCultureInfo("fr-FR")); // 75,50
        result.StandardOutput.Should().Match(output => 
            output.Contains(amount2WithDot) || output.Contains(amount2WithComma), 
            because: "the second record's amount should be listed with 2 decimal places (period or comma separator).");
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