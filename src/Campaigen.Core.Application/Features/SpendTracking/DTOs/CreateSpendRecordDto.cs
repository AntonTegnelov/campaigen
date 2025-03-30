namespace Campaigen.Core.Application.Features.SpendTracking.DTOs;

public class CreateSpendRecordDto
{
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    // Add validation attributes later if needed
} 