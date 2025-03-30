using Campaigen.Core.Application.Features.InfluencerManagement.Abstractions;
using Campaigen.Core.Application.Features.InfluencerManagement.DTOs;
using System.CommandLine;
using System.CommandLine.Invocation;
using System;
using System.Threading.Tasks;

namespace Campaigen.CLI.Commands;

public static class InfluencerCommands
{
    public static Command BuildAddInfluencerCommand()
    {
        var nameOption = new Option<string>(
            name: "--name",
            description: "The name of the influencer.") { IsRequired = true };
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

    public static Command BuildListInfluencerCommand()
    {
        return new ListInfluencerCommand();
    }

    // Command definition for "influencer add"
    public class AddInfluencerCommand : Command
    {
        public AddInfluencerCommand() : base("add", "Add a new influencer.") { }

        public new string Name { get; set; } = null!;
        public string? Handle { get; set; }
        public string? Platform { get; set; }
        public string? Niche { get; set; }
    }

    // Handler for "influencer add"
    public class AddInfluencerHandler : ICommandHandler
    {
        private readonly IInfluencerService _influencerService;
        public string Name { get; set; } = null!;
        public string? Handle { get; set; }
        public string? Platform { get; set; }
        public string? Niche { get; set; }

        public AddInfluencerHandler(IInfluencerService influencerService)
        {
            _influencerService = influencerService;
        }

        // Implement required synchronous Invoke
        public int Invoke(InvocationContext context)
        {
            return InvokeAsync(context).GetAwaiter().GetResult();
        }

        // Implement required asynchronous InvokeAsync
        public async Task<int> InvokeAsync(InvocationContext context)
        {
            var dto = new CreateInfluencerDto
            {
                Name = Name,
                Handle = Handle,
                Platform = Platform,
                Niche = Niche
            };

            Console.WriteLine($"Adding influencer: Name={dto.Name}, Handle={dto.Handle}, Platform={dto.Platform}, Niche={dto.Niche}");
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
                    Console.Error.WriteLine("Failed to create influencer.");
                    return 1; // Failure
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                return 1; // Failure
            }
        }
    }

    // Command definition for "influencer list"
    public class ListInfluencerCommand : Command
    {
        public ListInfluencerCommand() : base("list", "List all influencers.") { }
    }

    // Handler for "influencer list"
    public class ListInfluencerHandler : ICommandHandler
    {
        private readonly IInfluencerService _influencerService;

        public ListInfluencerHandler(IInfluencerService influencerService)
        {
            _influencerService = influencerService;
        }

        // Implement required synchronous Invoke
        public int Invoke(InvocationContext context)
        {
            return InvokeAsync(context).GetAwaiter().GetResult();
        }

        // Implement required asynchronous InvokeAsync
        public async Task<int> InvokeAsync(InvocationContext context)
        {
            Console.WriteLine("Listing all influencers...");
            try
            {
                var influencers = await _influencerService.ListInfluencersAsync();

                if (influencers != null && influencers.Any())
                {
                    foreach (var influencer in influencers)
                    {
                        Console.WriteLine($"ID: {influencer.Id}, Name: {influencer.Name}, Handle: {influencer.Handle}, Platform: {influencer.Platform}, Niche: {influencer.Niche}");
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
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                return 1; // Failure
            }
        }
    }
} 