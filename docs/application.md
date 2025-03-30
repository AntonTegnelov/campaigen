# Application Layer (`Campaigen.Core.Application`)

This layer acts as the orchestrator for the application's use cases. It defines the operations the application can perform and coordinates the interaction between the presentation layer and the domain/infrastructure layers.

## Key Responsibilities

- Implement application-specific business logic (use cases).
- Define interfaces for infrastructure concerns (like repositories) to achieve Dependency Inversion.
- Define Data Transfer Objects (DTOs) for communication with the presentation layer.
- Mediate data flow between layers.
- Handle application-level validation and error handling.

## Structure

- Organized by feature (Vertical Slice) under the `Features/` directory.
- Each feature typically contains:
  - `Abstractions/`: Interfaces (e.g., `I[Feature]Service`, `I[Feature]Repository`).
  - `DTOs/`: Data Transfer Objects (e.g., `[Feature]Dto`, `Create[Feature]Dto`).
  - `[Feature]Service.cs`: The implementation of the service interface.

## Core Components

### Influencer Management (`Features/InfluencerManagement`)

- **`Abstractions/IInfluencerService.cs`**: Defines methods for managing influencers (e.g., `CreateInfluencerAsync`, `GetInfluencerAsync`, `ListInfluencersAsync`).
- **`Abstractions/IInfluencerRepository.cs`**: Defines methods for data access related to influencers (e.g., `AddAsync`, `GetByIdAsync`, `GetAllAsync`, `UpdateAsync`, `DeleteAsync`). Implemented in the Infrastructure layer.
- **`DTOs/`**: Contains `InfluencerDto` (for returning data) and `CreateInfluencerDto` (for input).
- **`InfluencerService.cs`**: Implements `IInfluencerService`. Coordinates calls to `IInfluencerRepository`, maps between Domain entities (`Influencer`) and DTOs.

### Spend Tracking (`Features/SpendTracking`)

- **`Abstractions/ISpendTrackingService.cs`**: Defines methods for managing spend records (e.g., `CreateSpendRecordAsync`, `GetSpendRecordAsync`, `ListSpendRecordsAsync`).
- **`Abstractions/ISpendTrackingRepository.cs`**: Defines methods for data access related to spend records. Implemented in the Infrastructure layer.
- **`DTOs/`**: Contains `SpendRecordDto` and `CreateSpendRecordDto`.
- **`SpendTrackingService.cs`**: Implements `ISpendTrackingService`. Coordinates calls to `ISpendTrackingRepository`, maps between Domain entities (`SpendRecord`) and DTOs.

## Design Considerations (Current State & Potential Improvements)

- **Validation:** Currently lacks explicit input validation for DTOs. Add validation logic (e.g., using FluentValidation or data annotations) at the beginning of service methods to ensure data integrity before processing.
- **Error Handling:** Service methods currently return `Task<Dto?>`, using `null` to indicate potential failure. Consider using a more expressive `Result` pattern or specific exceptions to clearly communicate the nature of errors (e.g., validation error, not found, persistence error).
- **Mapping:** Uses simple private helper methods for mapping between Domain entities and DTOs. This is sufficient for now but could be replaced by libraries like AutoMapper if mapping logic becomes complex.
- **Use Case Logic:** Services currently perform basic CRUD orchestration. More complex business logic specific to a use case (but not belonging in the domain entity itself) should reside here.
