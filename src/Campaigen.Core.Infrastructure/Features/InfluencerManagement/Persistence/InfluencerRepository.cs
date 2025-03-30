using Campaigen.Core.Application.Features.InfluencerManagement.Abstractions;
using Campaigen.Core.Domain.Features.InfluencerManagement;
using Campaigen.Core.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq; // Required for LINQ methods like AsNoTracking

namespace Campaigen.Core.Infrastructure.Features.InfluencerManagement.Persistence;

/// <summary>
/// EF Core implementation of the <see cref="IInfluencerRepository"/>.
/// </summary>
public class InfluencerRepository : IInfluencerRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="InfluencerRepository"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public InfluencerRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task AddAsync(Influencer influencer)
    {
        // Note: Relying on EF Core to throw if _context.Influencers is null (only happens on context setup error).
        await _context.Influencers.AddAsync(influencer);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id)
    {
        var influencer = await _context.Influencers.FindAsync(id);
        if (influencer != null)
        {
            _context.Influencers.Remove(influencer);
            await _context.SaveChangesAsync();
        }
        // Note: No exception thrown if not found.
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Influencer>> GetAllAsync()
    {
        // AsNoTracking for read-only queries.
        return await _context.Influencers.AsNoTracking().ToListAsync();
    }

    /// <inheritdoc />
    public async Task<Influencer?> GetByIdAsync(Guid id)
    {
        // FindAsync is optimal for PK lookups.
        return await _context.Influencers.FindAsync(id);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Influencer influencer)
    {
        // Mark the entire entity as Modified.
        _context.Entry(influencer).State = EntityState.Modified;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            // Basic concurrency handling: Log and rethrow.
            // Consider more sophisticated strategies if needed (e.g., client wins, store wins).
            Console.Error.WriteLine($"Concurrency error updating influencer {influencer.Id}: {ex.Message}");
            throw;
        }
    }
} 