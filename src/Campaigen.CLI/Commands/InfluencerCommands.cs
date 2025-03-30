using Campaigen.Core.Application.Features.InfluencerManagement.Abstractions;
using Campaigen.Core.Application.Features.InfluencerManagement.DTOs;
using System.CommandLine;
using System.CommandLine.Invocation;
using System;
using System.Threading.Tasks;
using System.Linq; // Required for Any()

namespace Campaigen.CLI.Commands;

/// <summary>
/// Contains static methods for building influencer management related commands
/// and their corresponding handler classes.
/// </summary>
public static class InfluencerCommands
{
    /// <summary>
    /// Builds the "influencer add" command with its options.
    /// </summary>
    /// <returns>The configured "add" command.</returns>
    public static Command BuildAddInfluencerCommand()
    {
        var nameOption = new Option<string>(
            name: "--name",
            description: "The name of the influencer.")
        { IsRequired = true };
        var handleOption = new Option<string?>(
            name: "--handle",
            description: "The influencer's social media handle.");
        var platformOption = new Option<string?>(
            name: "--platform",
            description: "The primary platform (e.g., Instagram, TikTok).");
        var nicheOption = new Option<string?>(
            name: "--niche",
            description: "The influencer's niche or category.");

        var addCommand = new AddInfluencerCommand();
        addCommand.AddOption(nameOption);
        addCommand.AddOption(handleOption);
        addCommand.AddOption(platformOption);
        addCommand.AddOption(nicheOption);

        return addCommand;
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
    /// Represents the command definition for "influencer add".
    /// Used for model binding command line arguments.
    /// </summary>
    public class AddInfluencerCommand : Command
    {
        /// <summary>Initializes a new instance of the <see cref="AddInfluencerCommand"/> class.</summary>
        public AddInfluencerCommand() : base("add", "Add a new influencer.") { }

        /// <summary>Gets or sets the influencer's name.</summary>
        public new string Name { get; set; } = null!;
        /// <summary>Gets or sets the optional handle.</summary>
        public string? Handle { get; set; }
        /// <summary>Gets or sets the optional platform.</summary>
        public string? Platform { get; set; }
        /// <summary>Gets or sets the optional niche.</summary>
        public string? Niche { get; set; }
    }

    /// <summary>
    /// Handles the logic for the "influencer add" command.
    /// </summary>
    public class AddInfluencerHandler : ICommandHandler
    {
        private readonly IInfluencerService _influencerService;

        // Properties are bound from the command line arguments
        /// <summary>Gets or sets the influencer's name.</summary>
        public string Name { get; set; } = null!;
        /// <summary>Gets or sets the optional handle.</summary>
        public string? Handle { get; set; }
        /// <summary>Gets or sets the optional platform.</summary>
        public string? Platform { get; set; }
        /// <summary>Gets or sets the optional niche.</summary>
        public string? Niche { get; set; }

        /// <summary>Initializes a new instance of the <see cref="AddInfluencerHandler"/> class.</summary>
        /// <param name="influencerService">The injected influencer service.</param>
        public AddInfluencerHandler(IInfluencerService influencerService)
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
            var dto = new CreateInfluencerDto
            {
                Name = Name,
                Handle = Handle,
                Platform = Platform,
                Niche = Niche
            };

            Console.WriteLine($"Adding influencer: Name=\"{dto.Name}\", Handle={dto.Handle ?? "N/A"}, Platform={dto.Platform ?? "N/A"}, Niche={dto.Niche ?? "N/A"}");
            try
            {
                var result = await _influencerService.CreateInfluencerAsync(dto);
                if (result != null)
                {
                    Console.WriteLine($"Influencer created with ID: {result.Id}");
                    return 0; // Success
                }
                else
                {
                    Console.Error.WriteLine("Failed to create influencer (Service returned null).");
                    return 1; // Failure
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred during influencer creation: {ex.Message}");
                // Consider logging the full exception ex
                return 1; // Failure
            }
        }
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

                if (influencers != null && influencers.Any())
                {
                    // Simple table-like output
                    Console.WriteLine("\nID                                     Name                 Handle               Platform             Niche");
                    Console.WriteLine(new string('-', 100)); // Adjusted width
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