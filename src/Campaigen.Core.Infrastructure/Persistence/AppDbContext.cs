using Microsoft.EntityFrameworkCore;
using Campaigen.Core.Domain.Features.SpendTracking;
using Campaigen.Core.Domain.Features.InfluencerManagement;

namespace Campaigen.Core.Infrastructure.Persistence;

/// <summary>
/// Represents the application's database context using Entity Framework Core.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppDbContext"/> class.
    /// </summary>
    /// <param name="options">The options to be used by the DbContext.</param>
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // --- DbSets --- //

    /// <summary>
    /// Gets or sets the DbSet for <see cref="SpendRecord"/> entities.
    /// Use `null!` forgiving operator as EF Core ensures this is initialized.
    /// </summary>
    public DbSet<SpendRecord> SpendRecords { get; set; } = null!;

    /// <summary>
    /// Gets or sets the DbSet for <see cref="Influencer"/> entities.
    /// Use `null!` forgiving operator as EF Core ensures this is initialized.
    /// </summary>
    public DbSet<Influencer> Influencers { get; set; } = null!;

    // --- Model Configuration --- //

    // protected override void OnModelCreating(ModelBuilder modelBuilder)
    // {
    //     base.OnModelCreating(modelBuilder);
    //     // Configure entity relationships, constraints, value conversions, etc. here.
    //     // Example: modelBuilder.Entity<SpendRecord>().Property(p => p.Amount).HasPrecision(18, 2);
    // }
}