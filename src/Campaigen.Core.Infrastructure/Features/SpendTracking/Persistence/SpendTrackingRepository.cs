using Campaigen.Core.Application.Features.SpendTracking.Abstractions;
using Campaigen.Core.Domain.Features.SpendTracking;
using Campaigen.Core.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore; // Added for EF Core async methods

namespace Campaigen.Core.Infrastructure.Features.SpendTracking.Persistence;

public class SpendTrackingRepository : ISpendTrackingRepository
{
    private readonly AppDbContext _context;

    public SpendTrackingRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(SpendRecord spendRecord)
    {
        await _context.SpendRecords.AddAsync(spendRecord);
        await _context.SaveChangesAsync(); // Persist changes
    }

    public async Task DeleteAsync(Guid id)
    {
        var spendRecord = await _context.SpendRecords.FindAsync(id);
        if (spendRecord != null)
        {
            _context.SpendRecords.Remove(spendRecord);
            await _context.SaveChangesAsync();
        }
        // Consider if throwing an exception if not found is better
    }

    public async Task<IEnumerable<SpendRecord>> GetAllAsync()
    {
        // Use ToListAsync() for async enumeration
        return await _context.SpendRecords.AsNoTracking().ToListAsync();
        // AsNoTracking() is good practice for read-only queries
    }

    public async Task<SpendRecord?> GetByIdAsync(Guid id)
    {
        // FindAsync is suitable for finding by primary key
        return await _context.SpendRecords.FindAsync(id);
    }

    public async Task UpdateAsync(SpendRecord spendRecord)
    {
        _context.SpendRecords.Update(spendRecord); // Marks entity as modified
        await _context.SaveChangesAsync();
        // Note: Ensure the entity being passed is tracked or handle concurrency
    }
} 