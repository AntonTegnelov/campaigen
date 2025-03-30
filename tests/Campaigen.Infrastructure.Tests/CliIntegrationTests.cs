using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore; // Required for DbContextOptionsBuilder
using Campaigen.Core.Infrastructure.Persistence; // Required for AppDbContext

namespace Campaigen.Infrastructure.Tests;

// Note: These tests are true integration tests. They build (implicitly assume built)
// and run the CLI executable against a real (temporary) SQLite database.
public class CliIntegrationTests : IDisposable
{
    private readonly string _testDbPath;
    private readonly string _connectionString;
    private static readonly string _cliDllPath = FindCliDllPath();

    public CliIntegrationTests()
    {
        // Create a unique temporary SQLite database file for each test run
        _testDbPath = Path.Combine(Path.GetTempPath(), $"campaigen_test_{Guid.NewGuid()}.db");
        _connectionString = $"Data Source={_testDbPath}";

        // Ensure the database schema is created for this test database
        EnsureDatabaseCreated();
    }

    // Ensure the test database file is deleted after each test
    public void Dispose()
    {
        // Attempt to delete the test database file.
        // Add a small delay and retry logic as file handles might not be released immediately.
        for (int i = 0; i < 3; i++)
        {
            try
            {
                if (File.Exists(_testDbPath))
                {
                    File.Delete(_testDbPath);
                }
                return; // Exit if successful
            }
            catch (IOException) when (i < 2)
            {
                // Log or print warning on failed attempts (optional)
                // Console.WriteLine($"Warning: Attempt {i + 1} failed to delete test DB {_testDbPath}. Retrying... Error: {ex.Message}");
                Task.Delay(100).Wait(); // Wait 100ms before retrying
            }
            catch (Exception ex)
            {
                // Log or handle unexpected exceptions during delete
                Console.WriteLine($"Error deleting test DB {_testDbPath}: {ex.Message}");
                break; // Don't retry on other exceptions
            }
        }
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Helper method to find the path to the built CLI DLL.
    /// Assumes the solution has been built in Debug configuration.
    /// </summary>
    private static string FindCliDllPath()
    {
        // Try to navigate from the test DLL location up to the solution root and then down
        string testAssemblyPath = Assembly.GetExecutingAssembly().Location;
        // Expected structure: <repo_root>/tests/Campaigen.Infrastructure.Tests/bin/Debug/net8.0/Campaigen.Infrastructure.Tests.dll
        // We want:         <repo_root>/src/Campaigen.CLI/bin/Debug/net8.0/Campaigen.CLI.dll

        // Go up from DLL -> net8.0 -> Debug -> bin -> TestProjectDir -> tests -> SolutionDir
        DirectoryInfo? solutionDir = Directory.GetParent(testAssemblyPath)?.Parent?.Parent?.Parent?.Parent?.Parent;

        if (solutionDir == null || !solutionDir.Exists)
        {
            throw new DirectoryNotFoundException("Could not determine solution directory relative to test assembly.");
        }

        string cliDllPath = Path.Combine(solutionDir.FullName, "src", "Campaigen.CLI", "bin", "Debug", "net8.0", "Campaigen.CLI.dll");

        if (!File.Exists(cliDllPath))
        {
            // Fallback or alternative search paths could be added here if needed
            throw new FileNotFoundException($"CLI DLL not found at expected path: {cliDllPath}. Ensure the CLI project is built (Debug config).", cliDllPath);
        }
        return cliDllPath;
    }

    /// <summary>
    /// Creates the test database and applies migrations.
    /// </summary>
    private void EnsureDatabaseCreated()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connectionString)
            .Options;
        using var context = new AppDbContext(options);
        context.Database.EnsureCreated(); // Or use context.Database.Migrate(); if you have migrations
    }

    // Helper method to quote arguments if they contain spaces
    static string QuoteArgument(string arg)
    {
        // Basic quoting - might need refinement for more complex cases (e.g., args with quotes)
        return arg.Contains(' ') ? $"\"{arg}\"" : arg;
    }

    /// <summary>
    /// Executes the CLI application with specified arguments.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    /// <returns>A tuple containing ExitCode, StandardOutput, and StandardError.</returns>
    private async Task<(int ExitCode, string StdOut, string StdErr)> RunCliAsync(params string[] args)
    {
        var cliDllPath = FindCliDllPath();
        var cliDir = Path.GetDirectoryName(cliDllPath);
        if (cliDir == null)
        {
            throw new DirectoryNotFoundException($"Could not determine directory for CLI DLL: {cliDllPath}");
        }

        // Quote arguments containing spaces
        var quotedArgs = args.Select(QuoteArgument);

        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            // Use quoted arguments
            Arguments = $"{cliDllPath} {string.Join(" ", quotedArgs)}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = cliDir, // Set working directory to CLI output dir
            Environment =
            {
                // Override the connection string for the CLI process
                ["ConnectionStrings__DefaultConnection"] = _connectionString
            }
        };

        using var process = new Process();
        process.StartInfo = startInfo;

        process.Start();

        string stdout = await process.StandardOutput.ReadToEndAsync();
        string stderr = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        return (process.ExitCode, stdout, stderr);
    }

    // --- Test Cases --- //

    [Fact]
    public async Task SpendList_ShouldReturnEmpty_WhenDatabaseIsEmpty()
    {
        // Arrange & Act
        var (exitCode, stdOut, stdErr) = await RunCliAsync("spend", "list");

        // Assert
        exitCode.Should().Be(0);
        stdErr.Should().BeEmpty();
        stdOut.Should().Contain("Listing all spend records...");
        stdOut.Should().Contain("No spend records found.");
    }

    [Fact]
    public async Task SpendAdd_ShouldAddRecordAndReturnSuccess()
    {
        // Arrange
        string description = "CLI Test Spend";
        string category = "CLI Test";
        decimal amount = 12.34m;
        string date = "2024-03-30";

        // Act
        var (exitCode, stdOut, stdErr) = await RunCliAsync("spend", "add",
            "--amount", amount.ToString(),
            "--description", description,
            "--category", category,
            "--date", date);

        // Assert
        exitCode.Should().Be(0);
        stdErr.Should().BeEmpty();
        stdOut.Should().Contain("Adding spend:");
        stdOut.Should().Contain("Spend record created with ID:");

        // Optional: Verify directly in the database (demonstrates DB access in test)
        var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_connectionString).Options;
        using var context = new AppDbContext(options);
        var addedRecord = await context.SpendRecords.FirstOrDefaultAsync(r => r.Description == description);
        addedRecord.Should().NotBeNull();
        addedRecord!.Amount.Should().Be(amount);
        addedRecord.Category.Should().Be(category);
        addedRecord.Date.Date.Should().Be(new DateTime(2024, 3, 30));
    }

    [Fact]
    public async Task SpendList_ShouldReturnAddedRecord_AfterAdd()
    {
        // Arrange: Add a record first
        decimal amount = 98.76m;
        string desc = "Record for listing";
        var (addExitCode, _, addStdErr) = await RunCliAsync("spend", "add", "--amount", amount.ToString(), "--description", desc);
        addExitCode.Should().Be(0);
        addStdErr.Should().BeEmpty();

        // Act: List the records
        var (listExitCode, listStdOut, listStdErr) = await RunCliAsync("spend", "list");

        // Assert
        listExitCode.Should().Be(0);
        listStdErr.Should().BeEmpty();
        listStdOut.Should().Contain(amount.ToString());
        listStdOut.Should().Contain(desc);
        listStdOut.Should().NotContain("No spend records found.");
    }

    // TODO: Add tests for Influencer commands (add, list)
    // TODO: Add tests for error cases (e.g., missing required arguments, invalid data)
}