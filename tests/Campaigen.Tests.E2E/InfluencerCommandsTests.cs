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

    // TODO: Add test for 'influencer list'
    // TODO: Add tests for invalid input scenarios
} 