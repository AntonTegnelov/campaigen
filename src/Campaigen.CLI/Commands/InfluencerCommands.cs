using Campaigen.Core.Application.Features.InfluencerManagement.Abstractions;
using Campaigen.Core.Application.Features.InfluencerManagement.DTOs;
using System.CommandLine;
using System.CommandLine.Invocation;
using System;
using System.Threading.Tasks;
using System.Linq; // Required for Any()
using Microsoft.Extensions.DependencyInjection; // Add required using
using System.CommandLine.Parsing;
using System.CommandLine.Binding; // Required for Binder

namespace Campaigen.CLI.Commands;

/// <summary>
/// Contains static methods for building influencer management related commands
/// and their corresponding handler classes.
/// </summary>
public static class InfluencerCommands
{
    /// <summary>
    /// Builds the "influencer add" command.
    /// </summary>
    /// <returns>The configured "add" command instance.</returns>
    public static Command BuildAddInfluencerCommand()
    {
        return new AddInfluencerCommand();
    }

    /// <summary>
    /// Represents the command definition for "influencer add".
    /// </summary>
    public class AddInfluencerCommand : Command
    {
        // Define options directly within the command class
        public Option<string> NameOption { get; } = new Option<string>(
            name: "--influencer-name",
            description: "The name of the influencer.")
        { IsRequired = true };
        public Option<string?> HandleOption { get; } = new Option<string?>(
            name: "--handle",
            description: "The influencer's social media handle.");
        public Option<string?> PlatformOption { get; } = new Option<string?>(
            name: "--platform",
            description: "The primary platform (e.g., Instagram, TikTok).");
        public Option<string?> NicheOption { get; } = new Option<string?>(
            name: "--niche",
            description: "The influencer's niche or category.");

        /// <summary>Initializes a new instance of the <see cref="AddInfluencerCommand"/> class.</summary>
        public AddInfluencerCommand() : base("add", "Add a new influencer.")
        {
            // Add the locally defined options
            this.AddOption(NameOption);
            this.AddOption(HandleOption);
            this.AddOption(PlatformOption);
            this.AddOption(NicheOption);

            // Handler registration remains in Program.cs
        }
    }

    /// <summary>
    /// Handles the logic for the "influencer add" command.
    /// </summary>
    public class AddInfluencerHandler : ICommandHandler
    {
        private readonly IInfluencerService _influencerService;

        /// <summary>Initializes a new instance of the <see cref="AddInfluencerHandler"/> class.</summary>
        /// <param name="influencerService">The injected influencer service.</param>
        public AddInfluencerHandler(IInfluencerService influencerService)
        {
            _influencerService = influencerService ?? throw new ArgumentNullException(nameof(influencerService));
        }

        /// <summary>Invokes the handler synchronously.</summary>
        public int Invoke(InvocationContext context)
        {
            return InvokeAsync(context).GetAwaiter().GetResult();
        }

        /// <summary>Invokes the handler asynchronously.</summary>
        public async Task<int> InvokeAsync(InvocationContext context)
        {
            // Get the command instance to access its options
            var command = (AddInfluencerCommand)context.ParseResult.CommandResult.Command;

            // Get option values directly from the ParseResult
            var influencerName = context.ParseResult.GetValueForOption(command.NameOption);
            var handle = context.ParseResult.GetValueForOption(command.HandleOption);
            var platform = context.ParseResult.GetValueForOption(command.PlatformOption);
            var niche = context.ParseResult.GetValueForOption(command.NicheOption);

            // Create the DTO
            var dto = new CreateInfluencerDto
            {
                Name = influencerName,
                Handle = handle,
                Platform = platform,
                Niche = niche
            };

            try
            {
                // Call the service
                var result = await _influencerService.CreateInfluencerAsync(dto);
                
                // Always print success message
                Console.WriteLine("Influencer added successfully.");
                return 0;
            }
            catch (Exception ex)
            {
                // Handle errors
                Console.Error.WriteLine($"Error: {ex.Message}");
                return 1;
            }
        }
    }

    /// <summary>
    /// Builds the "influencer list" command.
    /// </summary>
    /// <returns>The configured "list" command.</returns>
    public static Command BuildListInfluencerCommand()
    {
        return new ListInfluencerCommand();
    }

    /// <summary>
    /// Represents the command definition for "influencer list".
    /// </summary>
    public class ListInfluencerCommand : Command
    {
        /// <summary>Initializes a new instance of the <see cref="ListInfluencerCommand"/> class.</summary>
        public ListInfluencerCommand() : base("list", "List all influencers.") { }
    }

    /// <summary>
    /// Handles the logic for the "influencer list" command.
    /// </summary>
    public class ListInfluencerHandler : ICommandHandler
    {
        private readonly IInfluencerService _influencerService;

        /// <summary>Initializes a new instance of the <see cref="ListInfluencerHandler"/> class.</summary>
        /// <param name="influencerService">The injected influencer service.</param>
        public ListInfluencerHandler(IInfluencerService influencerService)
        {
            _influencerService = influencerService;
        }

        /// <summary>Invokes the handler synchronously (required by ICommandHandler).</summary>
        public int Invoke(InvocationContext context)
        {
            return InvokeAsync(context).GetAwaiter().GetResult();
        }

        /// <summary>Invokes the handler asynchronously.</summary>
        public async Task<int> InvokeAsync(InvocationContext context)
        {
            Console.WriteLine("Listing all influencers...");
            try
            {
                var influencers = await _influencerService.ListInfluencersAsync();

                // Always display the headers
                Console.WriteLine("\nID                                     Name                 Handle               Platform             Niche");
                Console.WriteLine(new string('-', 100)); // Adjusted width
                
                if (influencers != null && influencers.Any())
                {
                    foreach (var influencer in influencers)
                    {
                        Console.WriteLine($"{influencer.Id,-37} {influencer.Name ?? "N/A",-20} {influencer.Handle ?? "N/A",-20} {influencer.Platform ?? "N/A",-20} {influencer.Niche ?? "N/A"}");
                    }
                }
                else
                {
                    Console.WriteLine("No influencers found.");
                }
                return 0; // Success
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred while listing influencers: {ex.Message}");
                // Consider logging the full exception ex
                return 1; // Failure
            }
        }
    }
}