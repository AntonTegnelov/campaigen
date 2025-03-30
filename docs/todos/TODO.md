# Campaigen - Detailed TODO List (Clean Architecture Approach)

This document outlines the specific tasks for building the Campaigen application using a Clean Architecture-inspired modular structure (`Domain`, `Application`, `Infrastructure`) with SOLID principles and Dependency Injection (DI), focusing on maximum modularity and loose coupling.

## Guiding Principles

- **Simplicity:** Start with the minimum viable features (Spend Tracking, Influencer Management).
- **Clean Architecture:** Strictly adhere to dependency rules (`Domain` <- `Application` <- `Infrastructure` <- `CLI`). No layer references a layer above it.
- **Async Consistency:** Ensure all potentially I/O-bound operations (database access, file system, network calls) are implemented asynchronously using `async`/`await` consistently throughout the application stack.
- **Modularity:** Organize code by feature within each layer.
- **Loose Coupling:** Minimize dependencies between features. Use DTOs to transfer data across layer boundaries, preventing leakage of domain or infrastructure details.
- **Dependency Injection:** Use DI heavily. Configure services and dependencies in `Campaigen.CLI/Program.cs`.

## Phase 1: Core Foundation & Setup

- **[Setup - Projects & Solution]**
  - [x] Create Solution (`Campaigen.sln`).
  - [x] Create `Campaigen.Core.Domain` classlib (`src/Campaigen.Core.Domain`).
  - [x] Create `Campaigen.Core.Application` classlib (`src/Campaigen.Core.Application`).
  - [x] Create `Campaigen.Core.Infrastructure` classlib (`src/Campaigen.Core.Infrastructure`).
  - [x] Create `Campaigen.CLI` console app (`src/Campaigen.CLI`).
  - [x] Add all projects to `Campaigen.sln`.
  - [x] Add reference: `Application` -> `Domain`.
  - [x] Add reference: `Infrastructure` -> `Application`.
  - [x] Add reference: `CLI` -> `Infrastructure`.
- **[Setup - Testing]**
  - [x] Create Unit Testing project (`dotnet new xunit -n Campaigen.Application.Tests -o tests/Campaigen.Application.Tests`).
  - [x] Add `Campaigen.Application.Tests` to solution (`dotnet sln add tests/Campaigen.Application.Tests/Campaigen.Application.Tests.csproj`).
  - [x] Add reference from `Campaigen.Application.Tests` to `Campaigen.Core.Application` (`dotnet add tests/Campaigen.Application.Tests/Campaigen.Application.Tests.csproj reference src/Campaigen.Core.Application/Campaigen.Core.Application.csproj`).
  - [x] Install Mocking Library (e.g., Moq) in `Campaigen.Application.Tests` (`dotnet add tests/Campaigen.Application.Tests/Campaigen.Application.Tests.csproj package Moq`).
  - [x] Install Assertion Library (e.g., FluentAssertions) in `Campaigen.Application.Tests` (`dotnet add tests/Campaigen.Application.Tests/Campaigen.Application.Tests.csproj package FluentAssertions`).
- **[Setup - Persistence (EF Core + SQLite)]**
  - [x] Add EF Core package to `Campaigen.Core.Infrastructure` (`dotnet add src/Campaigen.Core.Infrastructure/Campaigen.Core.Infrastructure.csproj package Microsoft.EntityFrameworkCore.Sqlite`).
  - [x] Add EF Core Design package to `Campaigen.CLI` (needed for migrations) (`dotnet add src/Campaigen.CLI/Campaigen.CLI.csproj package Microsoft.EntityFrameworkCore.Design`).
  - [x] Setup EF Core `AppDbContext` (`src/Campaigen.Core.Infrastructure/Persistence/AppDbContext.cs`).
    - [x] Define `DbSet` properties for upcoming domain models.
    - [x] Configure connection string reading (will be injected via DI).
  - [x] Configure DI for DbContext in `Campaigen.CLI/Program.cs` (using connection string from config).
- **[Setup - Configuration]**
  - [x] Setup Basic Configuration (`appsettings.json`) in `Campaigen.CLI` (`src/Campaigen.CLI/appsettings.json`) - Include `ConnectionStrings` section.
  - [x] Configure `Program.cs` to load `appsettings.json` (`src/Campaigen.CLI/Program.cs`).
- **[Setup - Initial Migration]**
  - [x] Create initial EF Core Migration (`dotnet ef migrations add InitialCreate --project src/Campaigen.Core.Infrastructure --startup-project src/Campaigen.CLI`). _(Run from workspace root)_
- **[Setup - Git]**
  - [x] Initialize Git repository (`git init`, initial commit).

## Phase 2: Feature - Marketing Spend Tracking

- **[Domain]** (`src/Campaigen.Core.Domain/Features/SpendTracking`)
  - [x] Define `SpendRecord` entity (`SpendRecord.cs`) - Properties: Id, Date, Amount, Description, Category. (Keep POCOs clean, no infrastructure concerns).
- **[Application - Abstractions]** (`src/Campaigen.Core.Application/Features/SpendTracking/Abstractions`)
  - [x] Define focused `ISpendTrackingRepository` interface (`ISpendTrackingRepository.cs`) - Methods: AddAsync, GetByIdAsync, GetAllAsync, UpdateAsync, DeleteAsync. (Ensure methods return `Task` or `Task<T>`). (ISP)
  - [x] Define focused `ISpendTrackingService` interface (`ISpendTrackingService.cs`) - Methods: CreateSpendRecordAsync(CreateSpendRecordDto dto), GetSpendRecordAsync(Guid id), ListSpendRecordsAsync(), etc. (Ensure methods return `Task` or `Task<T>`). (ISP)
- **[Application - DTOs]** (`src/Campaigen.Core.Application/Features/SpendTracking/DTOs`)
  - [x] Define `SpendRecordDto` (`SpendRecordDto.cs`) for returning data.
  - [x] Define `CreateSpendRecordDto` (`CreateSpendRecordDto.cs`) for input data.
- **[Application - Logic]** (`src/Campaigen.Core.Application/Features/SpendTracking`)
  - [x] Implement `SpendTrackingService` (`SpendTrackingService.cs`) - Use `ISpendTrackingRepository` asynchronously, map between Domain Entities and DTOs.
- **[Infrastructure - Persistence]** (`src/Campaigen.Core.Infrastructure/Features/SpendTracking/Persistence`)
  - [x] Implement `SpendTrackingRepository` using EF Core (`SpendTrackingRepository.cs`) - Implement interface methods asynchronously (e.g., using `await _context.SpendRecords.AddAsync(entity)`).
  - [x] Add `DbSet<SpendRecord>` to `AppDbContext` (`src/Campaigen.Core.Infrastructure/Persistence/AppDbContext.cs`).
  - [x] Create EF Core Migration for SpendRecord (`dotnet ef migrations add AddSpendRecord --project src/Campaigen.Core.Infrastructure --startup-project src/Campaigen.CLI`).
- **[DI Registration]** (`src/Campaigen.CLI/Program.cs`)
  - [x] Register `ISpendTrackingRepository` -> `SpendTrackingRepository`.
  - [x] Register `ISpendTrackingService` -> `SpendTrackingService`.
- **[Unit Tests]** (`tests/Campaigen.Application.Tests/Features/SpendTracking`)
  - [x] Write unit tests for `SpendTrackingService` (mock repository, verify DTO mapping).

## Phase 3: Feature - Influencer Management

- **[Domain]** (`src/Campaigen.Core.Domain/Features/InfluencerManagement`)
  - [x] Define `Influencer` entity (`Influencer.cs`) - Properties: Id, Name, Handle, Platform, Niche, etc.
- **[Application - Abstractions]** (`src/Campaigen.Core.Application/Features/InfluencerManagement/Abstractions`)
  - [x] Define focused `IInfluencerRepository` interface (`IInfluencerRepository.cs`) - Ensure methods return `Task` or `Task<T>`. (ISP)
  - [x] Define focused `IInfluencerService` interface (`IInfluencerService.cs`) - Ensure methods return `Task` or `Task<T>`. (ISP)
- **[Application - DTOs]** (`src/Campaigen.Core.Application/Features/InfluencerManagement/DTOs`)
  - [x] Define `InfluencerDto` (`InfluencerDto.cs`).
  - [x] Define `CreateInfluencerDto` (`CreateInfluencerDto.cs`).
- **[Application - Logic]** (`src/Campaigen.Core.Application/Features/InfluencerManagement`)
  - [x] Implement `InfluencerService` (`InfluencerService.cs`) - Use `IInfluencerRepository` asynchronously, map between Domain Entities and DTOs.
- **[Infrastructure - Persistence]** (`src/Campaigen.Core.Infrastructure/Features/InfluencerManagement/Persistence`)
  - [x] Implement `InfluencerRepository` (`InfluencerRepository.cs`) - Implement interface methods asynchronously.
  - [x] Add `DbSet<Influencer>` to `AppDbContext`.
  - [x] Create EF Core Migration for Influencer (`dotnet ef migrations add AddInfluencer ...`).
- **[DI Registration]** (`src/Campaigen.CLI/Program.cs`)
  - [x] Register `IInfluencerRepository` -> `InfluencerRepository`.
  - [x] Register `IInfluencerService` -> `InfluencerService`.
- **[Unit Tests]** (`tests/Campaigen.Application.Tests/Features/InfluencerManagement`)
  - [x] Write unit tests for `InfluencerService`.

## Phase 4: Command-Line Interface (`Campaigen.CLI`)

- **[Setup]**
  - [x] Install CLI Framework (`System.CommandLine`) (`dotnet add src/Campaigen.CLI/Campaigen.CLI.csproj package System.CommandLine`).
  - [x] Configure `System.CommandLine` basic setup in `src/Campaigen.CLI/Program.cs` (RootCommand, DI integration).
- **[Spend Tracking Commands]** (`src/Campaigen.CLI/Commands/SpendCommands.cs` or similar)
  - [x] Implement `spend add` command - Inject `ISpendTrackingService`, call service methods asynchronously, use `CreateSpendRecordDto`.
  - [x] Implement `spend list` command - Inject `ISpendTrackingService`, call service methods asynchronously, display results from `SpendRecordDto` list.
  - [x] Register spend commands with `RootCommand` in `Program.cs`.
- **[Influencer Management Commands]** (`src/Campaigen.CLI/Commands/InfluencerCommands.cs` or similar)
  - [x] Implement `influencer add` command - Inject `IInfluencerService`, call service methods asynchronously, use `CreateInfluencerDto`.
  - [x] Implement `influencer list` command - Inject `IInfluencerService`, call service methods asynchronously, display results from `InfluencerDto` list.
  - [x] Register influencer commands with `RootCommand` in `Program.cs`.
- **[Usability]**
  - [x] Implement basic error handling and user feedback (interact only with Application layer services).

## Phase 5: Refinement & Documentation

- [x] Write Integration Tests (e.g., Add `Campaigen.Infrastructure.Tests` project?). Test persistence layer or CLI commands against a test database.
  - [x] Setup Infrastructure Test project (`Campaigen.Infrastructure.Tests`).
  - [x] Implement basic tests for `SpendTrackingRepository`.
  - [x] Implement basic tests for `InfluencerRepository`.
  - [x] Implement tests for CLI commands (requires more setup).
- [x] Update `README.md` with actual usage instructions.
- [x] Add code comments.
- [x] Create `CONTRIBUTING.md`.
- [x] Choose and Add a `LICENSE` file.
- [x] Set up basic CI/CD pipeline (Build & Test).

## Phase 6: End-to-End (E2E) Testing

- **[Setup]**
  - [x] Create E2E Test Project (`dotnet new xunit -n Campaigen.Tests.E2E -o tests/Campaigen.Tests.E2E`).
  - [x] Add `Campaigen.Tests.E2E` to solution (`dotnet sln add tests/Campaigen.Tests.E2E/Campaigen.Tests.E2E.csproj`).
  - [x] Add reference from `Campaigen.Tests.E2E` to `Campaigen.CLI` (`dotnet add tests/Campaigen.Tests.E2E/Campaigen.Tests.E2E.csproj reference src/Campaigen.CLI/Campaigen.CLI.csproj`).
- **[Implementation]**
  - [x] Isolate Test Runs: Implement strategy for unique, temporary SQLite DB per test run (e.g., in test fixtures or base classes). Ensure cleanup.
  - [x] Database Setup Helper: Create utility to apply EF Core migrations to the test database before test execution.
  - [x] CLI Runner Helper: Create utility (`CliRunner.cs`?) to execute `Campaigen.CLI` with arguments, capture stdout/stderr, and return exit code.
- **[Test Cases]** (`tests/Campaigen.Tests.E2E`)
  - [x] Test `spend add`: Verify successful addition via output and direct DB check (optional).
  - [x] Test `spend list`: Verify output formatting and content after adding records.
  - [ ] Test `influencer add`: Verify successful addition. (Blocked by parsing issue)
  - [ ] Test `influencer list`: Verify output formatting and content. (Blocked by `influencer add`)
  - [x] Test Help Commands (`--help` for root and subcommands).
  - [ ] (Optional) Test Error Conditions: Invalid arguments (spend add tested), non-existent data lookups, etc.

## Deferred Features

- Social Media Monitoring
- AI-Powered Outreach
- Advanced Reporting/Analytics
- GUI

## Ongoing Tasks

- Code reviews, dependency updates, bug fixing.
