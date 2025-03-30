using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Campaigen.Core.Infrastructure.Persistence;
using Xunit;

namespace Campaigen.Tests.E2E;

/// <summary>
/// Base class for E2E tests providing isolated, temporary database environments.
/// Implements IDisposable for cleanup.
/// </summary>
public abstract class E2ETestBase : IDisposable
{
    protected readonly string TestDbPath;
    protected readonly string TestConnectionString;
    protected readonly Dictionary<string, string> TestEnvironmentVariables;

    protected E2ETestBase()
    {
        // Generate a unique path for the test database file
        TestDbPath = Path.Combine(Path.GetTempPath(), $"campaigen_e2e_{Guid.NewGuid()}.db");
        TestConnectionString = $"Data Source={TestDbPath}";

        // Environment variable format expected by .NET configuration for connection strings
        TestEnvironmentVariables = new Dictionary<string, string>
        {
            { "ConnectionStrings__DefaultConnection", TestConnectionString }
        };

        // Ensure the database is created and migrations are applied before tests run
        ApplyMigrations();
    }

    private void ApplyMigrations()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlite(TestConnectionString);

        // Important: Ensure the MigrationsAssembly is correctly specified,
        // matching the one used in the main application's DbContext configuration.
        optionsBuilder.UseSqlite(TestConnectionString, b =>
            b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));

        using var context = new AppDbContext(optionsBuilder.Options);
        try
        {
            // Ensure the directory exists (important for temporary paths)
            var dbDir = Path.GetDirectoryName(TestDbPath);
            if (dbDir != null && !Directory.Exists(dbDir))
            {
                Directory.CreateDirectory(dbDir);
            }

            // Create database and apply migrations
            context.Database.Migrate();
        }
        catch (Exception ex)
        {
            // Cleanup if migration fails to avoid leaving stray files
            Dispose();
            throw new InvalidOperationException($"Failed to apply migrations for test database '{TestDbPath}'. Error: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Runs the CLI with the test-specific environment variables (incl. connection string).
    /// </summary>
    protected async Task<CliResult> RunCliAsync(string arguments)
    {
        return await CliRunner.RunAsync(arguments, TestEnvironmentVariables);
    }

    /// <summary>
    /// Cleans up the temporary database file.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Attempt to delete the test database file
            // Use retries as the file might be temporarily locked by the CLI process ending
            int maxRetries = 5;
            int delayMs = 100;
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    if (File.Exists(TestDbPath))
                    {
                        // Ensure all connections are closed before deleting
                        SqliteConnection.ClearAllPools(); // Crucial for SQLite
                        File.Delete(TestDbPath);
                        // Console.WriteLine($"Deleted test database: {TestDbPath}"); // Debugging
                        break; // Exit loop if deletion succeeds
                    }
                    break; // Exit if file doesn't exist
                }
                catch (IOException ex) when (i < maxRetries - 1)
                {
                    // Console.WriteLine($"Cleanup attempt {i + 1} failed for {TestDbPath}, retrying... Error: {ex.Message}"); // Debugging
                    System.Threading.Thread.Sleep(delayMs);
                    delayMs *= 2; // Exponential backoff
                }
                catch (Exception ex)
                {
                    // Log or handle unexpected errors during cleanup
                    Console.WriteLine($"Error cleaning up test database '{TestDbPath}': {ex.Message}");
                    break; // Exit loop on other exceptions
                }
            }
        }
    }

    // Finalizer (just in case Dispose is not called)
    ~E2ETestBase()
    {
        Dispose(false);
    }
} 