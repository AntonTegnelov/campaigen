# Infrastructure Layer (`Campaigen.Core.Infrastructure`)

This layer is responsible for handling technical concerns and interactions with external systems, primarily data persistence.

## Key Responsibilities

- Implement data access logic (repositories) defined by interfaces in the Application layer.
- Interact with the database using an ORM (Entity Framework Core).
- Manage database schema changes (migrations).
- (Potentially) Interact with other external services (e.g., file storage, email, third-party APIs), although none are implemented currently.

## Structure

- **`Persistence/`**: Contains shared persistence components.
  - `AppDbContext.cs`: The main EF Core DbContext defining `DbSet` properties for domain entities and configuration.
- **`Migrations/`**: Contains EF Core generated migration files for tracking and applying database schema changes.
- **`Features/*/Persistence/`**: Contains feature-specific repository implementations.
  - `InfluencerManagement/Persistence/InfluencerRepository.cs`: Implements `IInfluencerRepository` using `AppDbContext`.
  - `SpendTracking/Persistence/SpendTrackingRepository.cs`: Implements `ISpendTrackingRepository` using `AppDbContext`.

## Core Components

- **`AppDbContext`**: Configured in `Program.cs` to use SQLite. Defines `DbSet<Influencer>` and `DbSet<SpendRecord>`.
- **Repositories (`InfluencerRepository`, `SpendTrackingRepository`)**: Implement standard CRUD operations (`AddAsync`, `GetByIdAsync`, `GetAllAsync`, `UpdateAsync`, `DeleteAsync`) using EF Core methods (`AddAsync`, `FindAsync`, `ToListAsync`, `Update`, `Remove`, `SaveChangesAsync`).

## Key Technologies

- **Entity Framework Core**: ORM used for database interaction.
- **Microsoft.EntityFrameworkCore.Sqlite**: Provider for SQLite database.

## Design Considerations (Current State & Potential Improvements)

- **Repository Pattern:** Correctly implements the repository pattern, hiding EF Core specifics from the Application layer.
- **Unit of Work:** EF Core's `DbContext` implicitly implements the Unit of Work pattern. `SaveChangesAsync()` is called within each repository method that modifies data. For operations involving multiple repositories that need to be atomic, a Unit of Work pattern could be explicitly implemented at the Application service level (injecting `AppDbContext` or a dedicated UoW interface) to control transaction boundaries more precisely.
- **Concurrency Handling:** Basic concurrency handling (`DbUpdateConcurrencyException`) is present in `InfluencerRepository`. This should be consistently applied and potentially enhanced (e.g., using row versioning) if concurrent updates are expected.
- **Read Optimizations:** `AsNoTracking()` is used for `GetAllAsync` queries, which is good for performance.
- **Error Handling:** Repository methods generally rely on EF Core exceptions. Specific infrastructure-related exceptions could be caught and wrapped if needed to provide more context to the Application layer.
