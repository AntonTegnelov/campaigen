namespace Campaigen.Core.Application.Features.SpendTracking.DTOs;

public class SpendRecordDto
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
} 