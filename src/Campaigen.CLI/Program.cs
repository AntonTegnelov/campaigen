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
                config.SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                      .AddEnvironmentVariables();
            });

            // Configure dependency injection services
            host.ConfigureServices((context, services) =>
            {
                // Configure EF Core DbContext with SQLite
                var connectionString = context.Configuration.GetConnectionString("DefaultConnection");
                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlite(connectionString,
                        // Specify the assembly where migrations are located (Infrastructure project)
                        b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

                // Register Application Services & Repositories using Scoped lifetime
                // Spend Tracking Feature
                services.AddScoped<ISpendTrackingRepository, SpendTrackingRepository>();
                services.AddScoped<ISpendTrackingService, SpendTrackingService>();

                // Influencer Management Feature
                services.AddScoped<IInfluencerRepository, InfluencerRepository>();
                services.AddScoped<IInfluencerService, InfluencerService>();

                // Add other application or infrastructure services here as needed
            });

            // Register command handlers with the host for DI
            // System.CommandLine.Hosting resolves the handler and injects dependencies
            host.UseCommandHandler<SpendCommands.AddSpendCommand, SpendCommands.AddSpendHandler>();
            host.UseCommandHandler<SpendCommands.ListSpendCommand, SpendCommands.ListSpendHandler>();
            // host.UseCommandHandler<InfluencerCommands.AddInfluencerCommand, InfluencerCommands.AddInfluencerHandler>(); // Handler is set directly
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
    // Build and add subcommands (add, list) from the SpendCommands class
    spendCommand.AddCommand(SpendCommands.BuildAddSpendCommand());
    spendCommand.AddCommand(SpendCommands.BuildListSpendCommand());

    // --- Influencer Command --- //
    var influencerCommand = new Command("influencer", "Manage influencer information.");
    // Build and add subcommands (add, list) from the InfluencerCommands class
    influencerCommand.AddCommand(InfluencerCommands.BuildAddInfluencerCommand());
    influencerCommand.AddCommand(InfluencerCommands.BuildListInfluencerCommand());

    // Add feature commands to the root command
    rootCommand.AddCommand(spendCommand);
    rootCommand.AddCommand(influencerCommand);

    return new CommandLineBuilder(rootCommand);
}

// Note: The original simple Console.WriteLine("Hello, World!"); has been replaced
// by the System.CommandLine setup.
