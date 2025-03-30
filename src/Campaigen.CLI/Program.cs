using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Parsing;
using Campaigen.Core.Infrastructure.Persistence;
using Campaigen.Core.Application.Features.SpendTracking.Abstractions;
using Campaigen.Core.Application.Features.SpendTracking;
using Campaigen.Core.Infrastructure.Features.SpendTracking.Persistence;
using Campaigen.Core.Application.Features.InfluencerManagement.Abstractions;
using Campaigen.Core.Application.Features.InfluencerManagement;
using Campaigen.Core.Infrastructure.Features.InfluencerManagement.Persistence;
using Campaigen.CLI.Commands;

// Main entry point for the CLI application
await BuildCommandLine()
    .UseHost(_ => Host.CreateDefaultBuilder(args), // Configure the generic host
        host =>
        {
            // Configure application settings
            host.ConfigureAppConfiguration((context, config) =>
            {
                // Load configuration from appsettings.json and environment variables
                config.SetBasePath(AppContext.BaseDirectory)
                      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                      .AddEnvironmentVariables();
            });

            // Configure dependency injection services
            host.ConfigureServices((context, services) =>
            {
                // Configure EF Core DbContext with SQLite
                var connectionString = context.Configuration.GetConnectionString("DefaultConnection");
                Console.WriteLine($"Using connection string: {connectionString}");
                
                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlite(connectionString,
                        // Specify the assembly where migrations are located (Infrastructure project)
                        b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

                // DEBUGGING: Create and apply migrations immediately to verify database setup
                var serviceProvider = services.BuildServiceProvider();
                try
                {
                    using (var scope = serviceProvider.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        // Ensure database is created and migrations are applied
                        dbContext.Database.EnsureCreated();
                        
                        // Check if tables exist
                        var tableCount = dbContext.Database.ExecuteSqlRaw("SELECT count(*) FROM sqlite_master WHERE type='table' AND (name='SpendRecords' OR name='Influencers');");
                        Console.WriteLine($"Number of tables found: {tableCount}");
                        
                        // Add a sample record for testing if none exist
                        if (!dbContext.SpendRecords.Any())
                        {
                            Console.WriteLine("Adding a sample spend record for testing...");
                            dbContext.SpendRecords.Add(new Campaigen.Core.Domain.Features.SpendTracking.SpendRecord
                            {
                                Id = Guid.NewGuid(),
                                Date = DateTime.UtcNow,
                                Amount = 100.0m,
                                Description = "Sample Test Record",
                                Category = "Test"
                            });
                            dbContext.SaveChanges();
                            Console.WriteLine("Sample record added successfully.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Database initialization error: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
                }

                // Register Application Services & Repositories using Scoped lifetime
                services.AddScoped<ISpendTrackingRepository, SpendTrackingRepository>();
                services.AddScoped<ISpendTrackingService, SpendTrackingService>();
                services.AddScoped<IInfluencerRepository, InfluencerRepository>();
                services.AddScoped<IInfluencerService, InfluencerService>();

                // Register Command Handlers for DI with the Host Builder
                // No need to register them separately if using UseCommandHandler below
            });

            // Register command handlers with the host for DI
            // System.CommandLine.Hosting resolves the handler and injects dependencies
            host.UseCommandHandler<SpendCommands.AddSpendCommand, SpendCommands.AddSpendHandler>();
            host.UseCommandHandler<SpendCommands.ListSpendCommand, SpendCommands.ListSpendHandler>();
            host.UseCommandHandler<InfluencerCommands.AddInfluencerCommand, InfluencerCommands.AddInfluencerHandler>();
            host.UseCommandHandler<InfluencerCommands.ListInfluencerCommand, InfluencerCommands.ListInfluencerHandler>();
        })
    .UseDefaults() // Enable standard middleware like help, version, etc.
    .Build() // Build the parser
    .InvokeAsync(args); // Invoke the command line processing

/// <summary>
/// Builds the command line structure using System.CommandLine.
/// </summary>
/// <returns>A CommandLineBuilder instance.</returns>
static CommandLineBuilder BuildCommandLine()
{
    var rootCommand = new RootCommand("Campaigen CLI for managing marketing campaign data.");

    // --- Spend Command --- //
    var spendCommand = new Command("spend", "Manage marketing spend records.");
    spendCommand.AddCommand(SpendCommands.BuildAddSpendCommand());
    spendCommand.AddCommand(SpendCommands.BuildListSpendCommand());

    // --- Influencer Command --- //
    var influencerCommand = new Command("influencer", "Manage influencer information.");
    influencerCommand.AddCommand(InfluencerCommands.BuildAddInfluencerCommand());
    influencerCommand.AddCommand(InfluencerCommands.BuildListInfluencerCommand());

    rootCommand.AddCommand(spendCommand);
    rootCommand.AddCommand(influencerCommand);

    return new CommandLineBuilder(rootCommand);
}

// Note: Manual handler setting removed, reverting to UseCommandHandler
