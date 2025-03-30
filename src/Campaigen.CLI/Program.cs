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

await BuildCommandLine()
    .UseHost(_ => Host.CreateDefaultBuilder(args),
        host =>
        {
            host.ConfigureAppConfiguration((context, config) =>
            {
                // Configure appsettings.json
                config.SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                      .AddEnvironmentVariables();
            });

            host.ConfigureServices((context, services) =>
            {
                // Configure EF Core DbContext
                var connectionString = context.Configuration.GetConnectionString("DefaultConnection");
                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlite(connectionString,
                        // Optional: Configure migrations assembly if it's different from Infrastructure
                        b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

                // Register Application Services & Repositories
                services.AddScoped<ISpendTrackingRepository, SpendTrackingRepository>();
                services.AddScoped<ISpendTrackingService, SpendTrackingService>();

                services.AddScoped<IInfluencerRepository, InfluencerRepository>();
                services.AddScoped<IInfluencerService, InfluencerService>();

                // Add other services as needed
            });

            host.UseCommandHandler<SpendCommands.AddSpendCommand, SpendCommands.AddSpendHandler>();
            host.UseCommandHandler<SpendCommands.ListSpendCommand, SpendCommands.ListSpendHandler>();
            host.UseCommandHandler<InfluencerCommands.AddInfluencerCommand, InfluencerCommands.AddInfluencerHandler>();
            host.UseCommandHandler<InfluencerCommands.ListInfluencerCommand, InfluencerCommands.ListInfluencerHandler>();
        })
    .UseDefaults()
    .Build()
    .InvokeAsync(args);

static CommandLineBuilder BuildCommandLine()
{
    var rootCommand = new RootCommand("Campaigen CLI for managing marketing campaign data.");

    // Manually create commands and add them (requires services from DI)
    // We need to resolve services within the command handlers now
    var spendCommand = new Command("spend", "Manage marketing spend records.");
    spendCommand.AddCommand(SpendCommands.BuildAddSpendCommand());
    spendCommand.AddCommand(SpendCommands.BuildListSpendCommand());

    var influencerCommand = new Command("influencer", "Manage influencer information.");
    influencerCommand.AddCommand(InfluencerCommands.BuildAddInfluencerCommand());
    influencerCommand.AddCommand(InfluencerCommands.BuildListInfluencerCommand());

    rootCommand.AddCommand(spendCommand);
    rootCommand.AddCommand(influencerCommand);

    return new CommandLineBuilder(rootCommand);
}


// Original basic console output - commented out or removed
// Console.WriteLine("Hello, World!");
