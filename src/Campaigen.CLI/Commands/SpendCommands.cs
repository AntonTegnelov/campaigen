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

            var dto = new CreateSpendRecordDto
            {
                Amount = amount, // Use value obtained directly
                Description = description,
                Category = category,
                Date = date ?? DateTime.UtcNow // Default to UtcNow if date not provided
            };

            Console.WriteLine($"Adding spend (Direct Parse): Amount={dto.Amount}, Desc={dto.Description ?? "N/A"}, Cat={dto.Category ?? "N/A"}, Date={dto.Date:yyyy-MM-dd}");
            try
            {
                var result = await _spendTrackingService.CreateSpendRecordAsync(dto);
                if (result != null)
                {
                    // Console.WriteLine($"Spend record created with ID: {result.Id}");
                    Console.WriteLine("Spend record added successfully."); // Use consistent success message
                    return 0; // Success exit code
                }
                else
                {
                    // This might happen if the service has validation that fails but doesn't throw
                    Console.Error.WriteLine("Failed to create spend record (Service returned null).");
                    return 1; // Failure exit code
                }
            }
            catch (Exception ex)
            {
                // Catch unexpected errors from the service/repository layer
                Console.Error.WriteLine($"An error occurred during spend record creation: {ex.Message}");
                // Consider logging the full exception ex
                return 1; // Failure exit code
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
                if (records != null && records.Any())
                {
                    // Simple table-like output
                    Console.WriteLine("\nID                                     Date        Amount  Category        Description");
                    Console.WriteLine(new string('-', 80));
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