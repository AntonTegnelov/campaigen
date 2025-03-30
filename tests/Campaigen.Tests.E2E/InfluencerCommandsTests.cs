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
        var handle = "@testinfluencer";
        var platform = "TestPlatform";
        var niche = "Testing";

        // Ensure arguments are quoted correctly for the CLI runner
        var arguments = $"influencer add --influencer-name \"{name}\" --handle \"{handle}\" --platform \"{platform}\" --niche \"{niche}\"";

        // Act: Run the CLI command using the helper from the base class
        var result = await RunCliAsync(arguments);

        // Assert - CLI Output
        result.ExitCode.Should().Be(0, because: $"the command should execute successfully. Error: {result.StandardError}");
        result.StandardOutput.Should().Contain("Influencer added successfully.", because: "the user should receive success feedback.");
        result.StandardError.Should().BeEmpty(because: "no errors should occur during successful addition.");

        // Assert - Database Verification
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlite(TestConnectionString); // Use the connection string from the base class
        using var verifyContext = new AppDbContext(optionsBuilder.Options);

        var addedRecord = await verifyContext.Influencers.FirstOrDefaultAsync(r => r.Name == name);

        addedRecord.Should().NotBeNull(because: "the record should have been added to the database.");
        addedRecord?.Handle.Should().Be(handle);
        addedRecord?.Platform.Should().Be(platform);
        addedRecord?.Niche.Should().Be(niche);
    }

    [Fact]
    public async Task InfluencerList_WhenRecordsExist_ShouldDisplayRecordsInTableFormat()
    {
        // Arrange: Add a couple of records first
        var record1Name = "List Influencer 1";
        var record1Handle = "@list1";
        var record1Platform = "Insta";
        var record1Niche = "List Niche A";
        await RunCliAsync($"influencer add --influencer-name \"{record1Name}\" --handle \"{record1Handle}\" --platform \"{record1Platform}\" --niche \"{record1Niche}\"");

        var record2Name = "List Influencer 2";
        var record2Handle = "@list2";
        var record2Platform = "TikTak";
        var record2Niche = "List Niche B";
        await RunCliAsync($"influencer add --influencer-name \"{record2Name}\" --handle \"{record2Handle}\" --platform \"{record2Platform}\" --niche \"{record2Niche}\"");

        // Act: Run the list command
        var result = await RunCliAsync("influencer list");

        // Assert
        result.ExitCode.Should().Be(0, because: $"'influencer list' should execute successfully. Error: {result.StandardError}");
        result.StandardError.Should().BeEmpty(because: "no errors should occur when listing.");

        // Verify headers
        result.StandardOutput.Should().Contain("ID", because: "the output table should have an ID header.");
        result.StandardOutput.Should().Contain("Name", because: "the output table should have a Name header.");
        result.StandardOutput.Should().Contain("Handle", because: "the output table should have a Handle header.");
        result.StandardOutput.Should().Contain("Platform", because: "the output table should have a Platform header.");
        result.StandardOutput.Should().Contain("Niche", because: "the output table should have a Niche header.");

        // Check for specific record details
        result.StandardOutput.Should().Contain(record1Name, because: "the first record's name should be listed.");
        result.StandardOutput.Should().Contain(record1Handle, because: "the first record's handle should be listed.");
        result.StandardOutput.Should().Contain(record1Platform, because: "the first record's platform should be listed.");
        result.StandardOutput.Should().Contain(record1Niche, because: "the first record's niche should be listed.");

        result.StandardOutput.Should().Contain(record2Name, because: "the second record's name should be listed.");
        result.StandardOutput.Should().Contain(record2Handle, because: "the second record's handle should be listed.");
        result.StandardOutput.Should().Contain(record2Platform, because: "the second record's platform should be listed.");
        result.StandardOutput.Should().Contain(record2Niche, because: "the second record's niche should be listed.");
    }

    // TODO: Add tests for invalid input scenarios
} 