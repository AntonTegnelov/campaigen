namespace Campaigen.Core.Domain.Features.SpendTracking;

public class SpendRecord
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
} 