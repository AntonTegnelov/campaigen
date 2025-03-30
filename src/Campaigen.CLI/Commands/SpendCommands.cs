using Campaigen.Core.Application.Features.SpendTracking.Abstractions;
using Campaigen.Core.Application.Features.SpendTracking.DTOs;
using System.CommandLine;
using System.CommandLine.Invocation;
using System;
using System.Threading.Tasks;
using System.Linq; // Required for Any()

namespace Campaigen.CLI.Commands;

/// <summary>
/// Contains static methods for building spend tracking related commands
/// and their corresponding handler classes.
/// </summary>
public static class SpendCommands
{
    /// <summary>
    /// Builds the "spend add" command.
    /// </summary>
    /// <returns>The configured "add" command.</returns>
    public static Command BuildAddSpendCommand()
    {
        // Return instance of the command class, options are defined within it
        return new AddSpendCommand();
    }

    /// <summary>
    /// Builds the "spend list" command.
    /// </summary>
    /// <returns>The configured "list" command.</returns>
    public static Command BuildListSpendCommand()
    {
        return new ListSpendCommand();
    }

    /// <summary>
    /// Represents the command definition for "spend add".
    /// </summary>
    public class AddSpendCommand : Command
    {
        // Define options directly within the command class
        public Option<decimal> AmountOption { get; } = new Option<decimal>(
            name: "--amount",
            description: "The amount spent.")
        { IsRequired = true };
        public Option<string?> DescriptionOption { get; } = new Option<string?>(
            name: "--description",
            description: "Description of the spend.");
        public Option<string?> CategoryOption { get; } = new Option<string?>(
            name: "--category",
            description: "Category of the spend.");
        public Option<DateTime?> DateOption { get; } = new Option<DateTime?>(
            name: "--date",
            description: "Date of the spend (defaults to today).");

        /// <summary>Initializes a new instance of the <see cref="AddSpendCommand"/> class.</summary>
        public AddSpendCommand() : base("add", "Add a new spend record.")
        {
            // Add the locally defined options
            this.AddOption(AmountOption);
            this.AddOption(DescriptionOption);
            this.AddOption(CategoryOption);
            this.AddOption(DateOption);
        }
    }

    /// <summary>
    /// Handles the logic for the "spend add" command.
    /// </summary>
    public class AddSpendHandler : ICommandHandler
    {
        private readonly ISpendTrackingService _spendTrackingService;

        /// <summary>Initializes a new instance of the <see cref="AddSpendHandler"/> class.</summary>
        /// <param name="spendTrackingService">The injected spend tracking service.</param>
        public AddSpendHandler(ISpendTrackingService spendTrackingService)
        {
            _spendTrackingService = spendTrackingService;
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
            var command = (AddSpendCommand)context.ParseResult.CommandResult.Command;

            // Get option values directly from the ParseResult
            var amount = context.ParseResult.GetValueForOption(command.AmountOption);
            var description = context.ParseResult.GetValueForOption(command.DescriptionOption);
            var category = context.ParseResult.GetValueForOption(command.CategoryOption);
            var date = context.ParseResult.GetValueForOption(command.DateOption);

            // Create the DTO
            var dto = new CreateSpendRecordDto
            {
                Amount = amount,
                Description = description,
                Category = category,
                Date = date ?? DateTime.UtcNow
            };

            try
            {
                // Call the service
                var result = await _spendTrackingService.CreateSpendRecordAsync(dto);
                
                // Always print success message on successful creation
                Console.WriteLine("Spend record added successfully.");
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
    /// Represents the command definition for "spend list".
    /// </summary>
    public class ListSpendCommand : Command
    {
        /// <summary>Initializes a new instance of the <see cref="ListSpendCommand"/> class.</summary>
        public ListSpendCommand() : base("list", "List all spend records.") { }
    }

    /// <summary>
    /// Handles the logic for the "spend list" command.
    /// </summary>
    public class ListSpendHandler : ICommandHandler
    {
        private readonly ISpendTrackingService _spendTrackingService;

        /// <summary>Initializes a new instance of the <see cref="ListSpendHandler"/> class.</summary>
        /// <param name="spendTrackingService">The injected spend tracking service.</param>
        public ListSpendHandler(ISpendTrackingService spendTrackingService)
        {
            _spendTrackingService = spendTrackingService;
        }

        /// <summary>Invokes the handler synchronously (required by ICommandHandler).</summary>
        public int Invoke(InvocationContext context)
        {
            return InvokeAsync(context).GetAwaiter().GetResult();
        }

        /// <summary>Invokes the handler asynchronously.</summary>
        public async Task<int> InvokeAsync(InvocationContext context)
        {
            Console.WriteLine("Listing all spend records...");
            try
            {
                var records = await _spendTrackingService.ListSpendRecordsAsync();
                
                // Always display the headers
                Console.WriteLine("\nID                                     Date        Amount  Category        Description");
                Console.WriteLine(new string('-', 80));
                
                if (records != null && records.Any())
                {
                    foreach (var record in records)
                    {
                        Console.WriteLine($"{record.Id,-37} {record.Date,-10:yyyy-MM-dd} {record.Amount,7:F2}  {record.Category ?? "N/A",-15} {record.Description ?? "N/A"}");
                    }
                }
                else
                {
                    Console.WriteLine("No spend records found.");
                }
                return 0; // Success
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred while listing spend records: {ex.Message}");
                // Consider logging the full exception ex
                return 1; // Failure
            }
        }
    }
}