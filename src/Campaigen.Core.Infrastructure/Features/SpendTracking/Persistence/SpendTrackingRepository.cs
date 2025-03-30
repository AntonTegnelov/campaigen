using Campaigen.Core.Application.Features.SpendTracking.Abstractions;
using Campaigen.Core.Domain.Features.SpendTracking;
using Campaigen.Core.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore; // Required for EF Core async methods like ToListAsync, FindAsync, etc.
using System.Linq; // Required for LINQ methods like AsNoTracking

namespace Campaigen.Core.Infrastructure.Features.SpendTracking.Persistence;

/// <summary>
/// EF Core implementation of the <see cref="ISpendTrackingRepository"/>.
/// </summary>
public class SpendTrackingRepository : ISpendTrackingRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpendTrackingRepository"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public SpendTrackingRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task AddAsync(SpendRecord spendRecord)
    {
        // EF Core's AddAsync starts tracking the entity in the Added state.
        await _context.SpendRecords.AddAsync(spendRecord);
        await _context.SaveChangesAsync(); // Persists changes to the database.
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id)
    {
        // FindAsync efficiently finds an entity by its primary key.
        var spendRecord = await _context.SpendRecords.FindAsync(id);
        if (spendRecord != null)
        {
            // Remove starts tracking the entity in the Deleted state.
            _context.SpendRecords.Remove(spendRecord);
            await _context.SaveChangesAsync(); // Persists the deletion.
        }
        // Note: No exception is thrown if the record is not found. Consider requirements.
    }

    /// <inheritdoc />
    public async Task<IEnumerable<SpendRecord>> GetAllAsync()
    {
        // AsNoTracking() improves performance for read-only queries as entities are not change-tracked.
        // ToListAsync() asynchronously executes the query and returns the list.
        return await _context.SpendRecords.AsNoTracking().ToListAsync();
    }

    /// <inheritdoc />
    public async Task<SpendRecord?> GetByIdAsync(Guid id)
    {
        // FindAsync is optimized for primary key lookups.
        // It first checks the local context cache before querying the database.
        return await _context.SpendRecords.FindAsync(id);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(SpendRecord spendRecord)
    {
        // Update marks the entire entity as Modified. Requires the entity to have a valid ID.
        // EF Core will generate an UPDATE statement for all properties.
        // Alternatively, attach and manually set property states for more control.
        _context.SpendRecords.Update(spendRecord);
        await _context.SaveChangesAsync();
        // Consider adding concurrency conflict handling (e.g., try-catch DbUpdateConcurrencyException).
    }
} 