using Campaigen.Core.Application.Features.InfluencerManagement.Abstractions;
using Campaigen.Core.Domain.Features.InfluencerManagement;
using Campaigen.Core.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Campaigen.Core.Infrastructure.Features.InfluencerManagement.Persistence;

public class InfluencerRepository : IInfluencerRepository
{
    private readonly AppDbContext _context;

    public InfluencerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Influencer influencer)
    {
        if (_context.Influencers == null)
        {
            throw new InvalidOperationException("Influencers DbSet is null.");
        }
        await _context.Influencers.AddAsync(influencer);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        if (_context.Influencers == null)
        {
            throw new InvalidOperationException("Influencers DbSet is null.");
        }
        var influencer = await _context.Influencers.FindAsync(id);
        if (influencer != null)
        {
            _context.Influencers.Remove(influencer);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Influencer>> GetAllAsync()
    {
        if (_context.Influencers == null)
        {
            return Enumerable.Empty<Influencer>();
        }
        return await _context.Influencers.ToListAsync();
    }

    public async Task<Influencer?> GetByIdAsync(Guid id)
    {
        if (_context.Influencers == null)
        {
            return null;
        }
        return await _context.Influencers.FindAsync(id);
    }

    public async Task UpdateAsync(Influencer influencer)
    {
        _context.Entry(influencer).State = EntityState.Modified;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            Console.Error.WriteLine($"Concurrency error updating influencer {influencer.Id}: {ex.Message}");
            throw;
        }
    }
} 