# Project Structure

The solution (`Campaigen.sln`) is organized into the following projects located within the `src/` directory:

- **`Campaigen.CLI`**: The main executable project containing the Command Line Interface.

  - References: `Campaigen.Core.Application`, `Campaigen.Core.Infrastructure` (for DI setup).
  - Key Components:
    - `Program.cs`: Application entry point, host configuration, DI setup, command registration.
    - `Commands/`: Contains command definitions and handlers (e.g., `SpendCommands.cs`, `InfluencerCommands.cs`) using `System.CommandLine`.
    - `appsettings.json`: Configuration file (e.g., database connection string).

- **`Campaigen.Core.Application`**: Contains application logic, service interfaces, DTOs, and defines repository interfaces.

  - References: `Campaigen.Core.Domain`.
  - Key Components:
    - `Features/*/`: Organized by feature (vertical slice).
    - `Features/*/Abstractions/`: Interfaces (`IService`, `IRepository`).
    - `Features/*/DTOs/`: Data Transfer Objects for communication.
    - `Features/*/`: Service implementations (`*Service.cs`).

- **`Campaigen.Core.Domain`**: Contains core domain entities and potentially domain logic/validation.

  - References: None (should not depend on other layers).
  - Key Components:
    - `Features/*/`: Organized by feature (vertical slice).
    - `Features/*/*.cs`: Domain entities (e.g., `Influencer.cs`, `SpendRecord.cs`).

- **`Campaigen.Core.Infrastructure`**: Implements interfaces defined in the Application layer, handling external concerns like database access.
  - References: `Campaigen.Core.Application`, `Campaigen.Core.Domain`.
  - Key Components:
    - `Persistence/AppDbContext.cs`: Entity Framework Core database context.
    - `Migrations/`: EF Core database migrations.
    - `Features/*/Persistence/`: Repository implementations (e.g., `*Repository.cs`).

See the [Architecture](architecture.md) document for how these projects interact.
