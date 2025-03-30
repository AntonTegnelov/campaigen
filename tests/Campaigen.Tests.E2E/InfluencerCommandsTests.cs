using System;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Campaigen.Core.Infrastructure.Persistence;

namespace Campaigen.Tests.E2E;

/// <summary>
/// E2E tests for the 'influencer' commands in the CLI.
/// </summary>
public class InfluencerCommandsTests : E2ETestBase // Inherit from the base class
{
    [Fact]
    public async Task InfluencerAdd_WithValidData_ShouldSucceedAndAddRecord()
    {
        // Arrange
        var name = "Test Influencer";
        var handle = "testhandle";
        var platform = "Instagram";
        var niche = "Test Niche";

        // Pass arguments as a string array
        var args = new[] {
            "influencer", "add",
            "--influencer-name", name,
            "--handle", handle,
            "--platform", platform,
            "--niche", niche
        };

        // Act: Run the CLI command using the helper from the base class
        var result = await RunCliAsync(args);

        // Assert - CLI Output
        result.ExitCode.Should().Be(0, because: $"the command should execute successfully. Error: {result.StandardError}");
        result.StandardOutput.Should().Contain("Influencer added successfully", because: "the user should receive success feedback.");
        result.StandardError.Should().BeEmpty(because: "no errors should occur during successful addition.");

        // Assert - Database Verification
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlite(TestConnectionString);
        using var verifyContext = new AppDbContext(optionsBuilder.Options);

        var addedRecord = await verifyContext.Influencers.FirstOrDefaultAsync(i => i.Handle == handle);

        addedRecord.Should().NotBeNull(because: "the record should have been added to the database.");
        if (addedRecord != null)
        {
            addedRecord.Name.Should().Be(name);
            addedRecord.Platform.Should().Be(platform);
            addedRecord.Niche.Should().Be(niche);
        }
    }

    [Fact]
    public async Task InfluencerList_WhenRecordsExist_ShouldDisplayRecordsInTableFormat()
    {
        // Arrange: Add a couple of records directly using the database helper
        var dbHelper = new DatabaseHelper(TestConnectionString);

        var name1 = "List Influencer A";
        var handle1 = "handleA";
        var platform1 = "YouTube";
        var niche1 = "Gaming";
        await dbHelper.AddInfluencerAsync(name1, handle1, platform1, niche1);

        var name2 = "List Influencer B";
        var handle2 = "handleB";
        var platform2 = "Instagram";
        var niche2 = "Fashion";
        await dbHelper.AddInfluencerAsync(name2, handle2, platform2, niche2);

        // Act: Run the list command
        var result = await RunCliAsync("influencer", "list");

        // Assert
        result.ExitCode.Should().Be(0, because: $"'influencer list' should execute successfully. Error: {result.StandardError}");
        result.StandardError.Should().BeEmpty(because: "no errors should occur when listing.");

        // Verify headers and the presence of both records in the output
        result.StandardOutput.Should().Contain("ID", because: "the output table should have an ID header.");
        result.StandardOutput.Should().Contain("Name", because: "the output table should have a Name header.");
        result.StandardOutput.Should().Contain("Handle", because: "the output table should have a Handle header.");
        result.StandardOutput.Should().Contain("Platform", because: "the output table should have a Platform header.");
        result.StandardOutput.Should().Contain("Niche", because: "the output table should have a Niche header.");

        // Check for specific record details
        result.StandardOutput.Should().Contain(name1, because: "the first record's name should be listed.");
        result.StandardOutput.Should().Contain(handle1, because: "the first record's handle should be listed.");
        result.StandardOutput.Should().Contain(platform1, because: "the first record's platform should be listed.");
        result.StandardOutput.Should().Contain(niche1, because: "the first record's niche should be listed.");

        result.StandardOutput.Should().Contain(name2, because: "the second record's name should be listed.");
        result.StandardOutput.Should().Contain(handle2, because: "the second record's handle should be listed.");
        result.StandardOutput.Should().Contain(platform2, because: "the second record's platform should be listed.");
        result.StandardOutput.Should().Contain(niche2, because: "the second record's niche should be listed.");
    }

    [Theory]
    [InlineData("influencer", "--help")]
    [InlineData("influencer", "add", "--help")]
    [InlineData("influencer", "list", "--help")]
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

    // TODO: Add tests for invalid input scenarios (missing required parameters, etc.)
}