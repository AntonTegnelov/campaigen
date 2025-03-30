using Campaigen.Core.Application.Features.SpendTracking.Abstractions;
using Campaigen.Core.Application.Features.SpendTracking.DTOs;
using System.CommandLine;
using System.CommandLine.Invocation;
using System;
using System.Threading.Tasks;

namespace Campaigen.CLI.Commands;

// Static class to build the commands, keeping Program.cs cleaner
public static class SpendCommands
{
    public static Command BuildAddSpendCommand()
    {
        var amountOption = new Option<decimal>(
            name: "--amount",
            description: "The amount spent.") { IsRequired = true };
        var descriptionOption = new Option<string?>(
            name: "--description",
            description: "Description of the spend.");
        var categoryOption = new Option<string?>(
            name: "--category",
            description: "Category of the spend.");
        var dateOption = new Option<DateTime?>(
            name: "--date",
            description: "Date of the spend (defaults to today).");

        var addCommand = new AddSpendCommand();
        addCommand.AddOption(amountOption);
        addCommand.AddOption(descriptionOption);
        addCommand.AddOption(categoryOption);
        addCommand.AddOption(dateOption);

        return addCommand;
    }

    public static Command BuildListSpendCommand()
    {
        return new ListSpendCommand();
    }

    // Command definition for "spend add"
    public class AddSpendCommand : Command
    {
        public AddSpendCommand() : base("add", "Add a new spend record.") { }

        public decimal Amount { get; set; }
        public new string? Description { get; set; }
        public string? Category { get; set; }
        public DateTime? Date { get; set; }
    }

    // Handler for "spend add"
    public class AddSpendHandler : ICommandHandler
    {
        private readonly ISpendTrackingService _spendTrackingService;
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public DateTime? Date { get; set; }

        public AddSpendHandler(ISpendTrackingService spendTrackingService)
        {
            _spendTrackingService = spendTrackingService;
        }

        // Implement required synchronous Invoke
        public int Invoke(InvocationContext context)
        {
            return InvokeAsync(context).GetAwaiter().GetResult();
        }

        // Implement required asynchronous InvokeAsync
        public async Task<int> InvokeAsync(InvocationContext context)
        {
            var dto = new CreateSpendRecordDto
            {
                Amount = Amount,
                Description = Description,
                Category = Category,
                Date = Date ?? DateTime.UtcNow
            };

            Console.WriteLine($"Adding spend: Amount={dto.Amount}, Desc={dto.Description}, Cat={dto.Category}, Date={dto.Date}");
            try
            {
                var result = await _spendTrackingService.CreateSpendRecordAsync(dto);
                if (result != null)
                {
                    Console.WriteLine($"Spend record created with ID: {result.Id}");
                    return 0; // Success
                }
                else
                {
                    Console.Error.WriteLine("Failed to create spend record.");
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

    // Command definition for "spend list"
    public class ListSpendCommand : Command
    {
        public ListSpendCommand() : base("list", "List all spend records.") { }
    }

    // Handler for "spend list"
    public class ListSpendHandler : ICommandHandler
    {
        private readonly ISpendTrackingService _spendTrackingService;

        public ListSpendHandler(ISpendTrackingService spendTrackingService)
        {
            _spendTrackingService = spendTrackingService;
        }

        // Implement required synchronous Invoke
        public int Invoke(InvocationContext context)
        {
            return InvokeAsync(context).GetAwaiter().GetResult();
        }

        // Implement required asynchronous InvokeAsync
        public async Task<int> InvokeAsync(InvocationContext context)
        {
            Console.WriteLine("Listing all spend records...");
            try
            {
                var records = await _spendTrackingService.ListSpendRecordsAsync();
                if (records != null && records.Any())
                {
                    foreach (var record in records)
                    {
                        Console.WriteLine($"ID: {record.Id}, Date: {record.Date.ToShortDateString()}, Amount: {record.Amount}, Desc: {record.Description}, Cat: {record.Category}");
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
                 Console.Error.WriteLine($"An error occurred: {ex.Message}");
                 return 1; // Failure
            }
        }
    }
} 