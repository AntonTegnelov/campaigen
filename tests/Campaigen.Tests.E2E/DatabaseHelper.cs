using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Campaigen.Core.Domain.Features.SpendTracking;
using Campaigen.Core.Domain.Features.InfluencerManagement;
using Campaigen.Core.Infrastructure.Persistence;

namespace Campaigen.Tests.E2E
{
    /// <summary>
    /// Helper class for directly manipulating the database in E2E tests.
    /// </summary>
    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        /// <summary>
        /// Directly adds a spend record to the database for testing.
        /// </summary>
        public async Task<SpendRecord> AddSpendRecordAsync(decimal amount, string description, string category, DateTime? date = null)
        {
            var record = new SpendRecord
            {
                Id = Guid.NewGuid(),
                Amount = amount,
                Description = description,
                Category = category,
                Date = date ?? DateTime.UtcNow
            };

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlite(_connectionString);
            
            using var context = new AppDbContext(optionsBuilder.Options);
            await context.SpendRecords.AddAsync(record);
            await context.SaveChangesAsync();

            return record;
        }

        /// <summary>
        /// Directly adds an influencer to the database for testing.
        /// </summary>
        public async Task<Influencer> AddInfluencerAsync(string name, string handle, string platform, string niche)
        {
            var influencer = new Influencer
            {
                Id = Guid.NewGuid(),
                Name = name,
                Handle = handle,
                Platform = platform,
                Niche = niche
            };

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlite(_connectionString);
            
            using var context = new AppDbContext(optionsBuilder.Options);
            await context.Influencers.AddAsync(influencer);
            await context.SaveChangesAsync();

            return influencer;
        }
    }
} 