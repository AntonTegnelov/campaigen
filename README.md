# Campaigen - Marketing Campaign Management CLI

**Note:** This project is under active development.

A .NET 8 CLI application designed to help manage marketing campaign data, focusing initially on spend tracking and influencer management.

## Overview

Campaigen provides command-line tools to record and view marketing spend and influencer details. It uses a Clean Architecture approach and Entity Framework Core with SQLite for persistence.

## Implemented Features

- **Marketing Spend Tracking:** Add and list marketing spend records.
- **Influencer Management:** Add and list influencer details.

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Setup & Configuration

1.  **Clone the repository:**
    ```bash
    git clone <your-repository-url>
    cd campaigen
    ```
2.  **Database Setup:**

    - The application uses SQLite. The database file (`campaigen.db` by default) will be created automatically in the output directory (`src/Campaigen.CLI/bin/Debug/net8.0/`) when the application runs for the first time after a migration.
    - The connection string is configured in `src/Campaigen.CLI/appsettings.json`. You can modify the database path there if needed.
    - **Apply Migrations:** Ensure the database schema is up-to-date by applying EF Core migrations. Run the following command from the **repository root**:
      ```powershell
      dotnet ef database update --project src/Campaigen.Core.Infrastructure --startup-project src/Campaigen.CLI
      ```
      _(Note: If you modify domain models later, you'll need to add a new migration (`dotnet ef migrations add ...`) and run `database update` again.)_

3.  **Build the project:**
    ```powershell
    dotnet build
    ```

### Usage

The main entry point is the `Campaigen.CLI` project. You can run commands using `dotnet run --project src/Campaigen.CLI/Campaigen.CLI.csproj -- [command] [options]...` from the repository root.

Alternatively, navigate to the CLI project directory (`cd src/Campaigen.CLI`) and use `dotnet run -- [command] [options]...`.

The `--` is important to separate `dotnet run` options from the application's command-line arguments.

**Examples:**

- **Get Help:**

  ```powershell
  # From root directory
  dotnet run --project src/Campaigen.CLI/Campaigen.CLI.csproj -- --help

  # From src/Campaigen.CLI directory
  dotnet run -- --help
  ```

- **Add a Spend Record:**

  ```powershell
  # Minimal: Amount is required
  dotnet run --project src/Campaigen.CLI/Campaigen.CLI.csproj -- spend add --amount 123.45

  # With optional details
  dotnet run --project src/Campaigen.CLI/Campaigen.CLI.csproj -- spend add --amount 99.99 --description "Facebook Ads Q1" --category "Social Media" --date 2024-01-15
  ```

- **List Spend Records:**

  ```powershell
  dotnet run --project src/Campaigen.CLI/Campaigen.CLI.csproj -- spend list
  ```

- **Add an Influencer:**

  ```powershell
  # Minimal: Name is required
  dotnet run --project src/Campaigen.CLI/Campaigen.CLI.csproj -- influencer add --name "TechReviewer"

  # With optional details
  dotnet run --project src/Campaigen.CLI/Campaigen.CLI.csproj -- influencer add --name "Lifestyle Guru" --handle "@lifestyleguru" --platform "Instagram" --niche "Lifestyle"
  ```

- **List Influencers:**
  ```powershell
  dotnet run --project src/Campaigen.CLI/Campaigen.CLI.csproj -- influencer list
  ```

## Contributing

Contributions are welcome! Please create an issue to discuss potential changes before submitting pull requests.

## License

(Consider adding a LICENSE file - e.g., MIT)
