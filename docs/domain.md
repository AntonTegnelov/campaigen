# Domain Layer (`Campaigen.Core.Domain`)

This layer represents the heart of the application, containing the core business concepts and rules.

## Key Responsibilities

- Define the essential entities and value objects of the business domain.
- Encapsulate business logic and validation rules that are intrinsic to the domain concepts.
- Remain independent of other layers (Application, Infrastructure, Presentation).

## Structure

- Organized by feature (Vertical Slice) under the `Features/` directory.

## Core Entities

1.  **`Features/InfluencerManagement/Influencer.cs`**

    - Represents a marketing influencer.
    - **Properties:**
      - `Id` (Guid): Unique identifier.
      - `Name` (string?): Influencer's name.
      - `Handle` (string?): Social media handle (e.g., @username).
      - `Platform` (string?): Primary social media platform.
      - `Niche` (string?): Content niche or category.
    - _Note: Currently implemented as a simple POCO. Could be enhanced with validation (e.g., required fields, handle format) and immutability._

2.  **`Features/SpendTracking/SpendRecord.cs`**
    - Represents a single record of marketing expenditure.
    - **Properties:**
      - `Id` (Guid): Unique identifier.
      - `Date` (DateTime): Date the spend occurred.
      - `Amount` (decimal): Monetary amount spent.
      - `Description` (string?): Optional description.
      - `Category` (string?): Optional category (e.g., Advertising, Software).
    - _Note: Currently implemented as a simple POCO. Could be enhanced with validation (e.g., non-negative Amount) and immutability._

## Design Considerations (Current State & Potential Improvements)

- **Anemic Domain Model:** The current entities are primarily data containers. Business logic and validation mostly reside in the Application layer. Consider moving intrinsic validation and behavior into the domain entities to create a richer domain model, improving encapsulation and robustness.
- **Immutability:** Properties currently have public setters. Consider using `init` setters or private setters with constructors/factory methods to enforce immutability, making the domain state more predictable.
- **Validation:** Add validation logic within constructors or factory methods to ensure entities are always created in a valid state (e.g., required fields are provided, `Amount` is positive).
