# Architecture

The Campaigen application follows a layered architecture combined with Vertical Slice Architecture principles to promote separation of concerns, maintainability, and scalability.

## Layers

1.  **Domain Layer (`Campaigen.Core.Domain`)**: Contains the core business logic, entities, and domain events. It has no dependencies on other layers.

    - **Entities**: Represent the core concepts (e.g., `Influencer`, `SpendRecord`). Ideally, these contain intrinsic validation and behavior.

2.  **Application Layer (`Campaigen.Core.Application`)**: Orchestrates the use cases of the application. It depends on the Domain layer.

    - **Services**: Implement application-specific logic (e.g., `InfluencerService`, `SpendTrackingService`). They coordinate interactions between the presentation layer and the domain/infrastructure layers.
    - **DTOs (Data Transfer Objects)**: Define the data structures used for communication between layers, particularly between the presentation layer and the Application layer.
    - **Interfaces**: Defines contracts (e.g., `IInfluencerRepository`, `ISpendTrackingRepository`, `IInfluencerService`) that are implemented by other layers (primarily Infrastructure).

3.  **Infrastructure Layer (`Campaigen.Core.Infrastructure`)**: Handles technical concerns like data persistence, external API interactions, logging, etc. It depends on the Application layer (for interfaces) and the Domain layer (for entities).

    - **Persistence**: Implements data access logic, often using an ORM like Entity Framework Core (`AppDbContext`, Repositories).
    - **Migrations**: Database schema changes.

4.  **Presentation Layer (`Campaigen.CLI`)**: Provides the user interface. In this case, it's a Command Line Interface (CLI). It depends on the Application layer.
    - **Commands**: Defines the structure and handlers for CLI commands using `System.CommandLine`.
    - **Configuration**: Entry point (`Program.cs`) for setting up hosting, dependency injection, configuration, and wiring up the application.

## Vertical Slicing

Within the Core layers (Domain, Application, Infrastructure), code is further organized by feature (or "vertical slice"). For example, all code related to `InfluencerManagement` (its domain entity, application service/DTOs/interfaces, repository implementation) resides in respective `Features/InfluencerManagement` folders within each layer's project.

This approach keeps related code cohesive and makes it easier to understand and modify a specific feature without navigating across broad layer-wide concerns.

## Key Principles

- **Separation of Concerns**: Each layer has distinct responsibilities.
- **Dependency Rule**: Dependencies flow inwards (Presentation -> Application -> Domain, Infrastructure -> Application/Domain).
- **Dependency Inversion**: Application layer defines interfaces implemented by Infrastructure, decoupling application logic from specific infrastructure implementations.
- **Explicit Dependencies**: Dependencies are injected via constructors using `Microsoft.Extensions.DependencyInjection`.
