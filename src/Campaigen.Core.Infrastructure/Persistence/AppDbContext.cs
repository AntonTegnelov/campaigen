using Microsoft.EntityFrameworkCore;
using Campaigen.Core.Domain.Features.SpendTracking;
using Campaigen.Core.Domain.Features.InfluencerManagement;

namespace Campaigen.Core.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // DbSet properties for entities will be added here later
    public DbSet<SpendRecord> SpendRecords { get; set; } = null!;
    public DbSet<Influencer> Influencers { get; set; } = null!;

    // protected override void OnModelCreating(ModelBuilder modelBuilder)
    // {
    //     base.OnModelCreating(modelBuilder);
    //     // Configure entity relationships and constraints here if needed
    // }
} 