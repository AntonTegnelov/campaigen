# Getting Started

This guide provides instructions on how to set up, build, configure, and run the Campaigen application.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later.
- A compatible development environment (e.g., Visual Studio, VS Code, Rider).

## Setup

1.  **Clone the Repository:**

    ```bash
    git clone <repository-url>
    cd campaigen
    ```

2.  **Restore Dependencies:**
    Navigate to the solution root directory (where `Campaigen.sln` is located) and run:
    ```bash
    dotnet restore
    ```

## Configuration

The application uses a SQLite database by default. The connection string is configured in `src/Campaigen.CLI/appsettings.json`.

```json
{
  "ConnectionStrings": {
    // Defines the path to the SQLite database file.
    // Update the path if necessary.
    "DefaultConnection": "Data Source=..\\..\\..\\campaigen.db"
  },
  "Logging": {
    /* ... logging configuration ... */
  }
}
```

- The default path `..\\..\\..\\campaigen.db` places the database file (`campaigen.db`) in the solution root directory when running from the CLI project's build output directory.
- Adjust the `Data Source` value if you prefer a different location.

## Database Migrations

The application uses EF Core Migrations to manage the database schema.

1.  **Ensure EF Core Tools are installed:**
    If you haven't already, install the EF Core command-line tools:

    ```bash
    dotnet tool install --global dotnet-ef
    ```

    Or update them:

    ```bash
    dotnet tool update --global dotnet-ef
    ```

2.  **Apply Initial Migration (or subsequent migrations):**
    Navigate to the **Infrastructure project directory** (`src/Campaigen.Core.Infrastructure`) and run the following command. You need to specify the startup project (`Campaigen.CLI`) which contains the configuration:

    ```bash
    dotnet ef database update --startup-project ../Campaigen.CLI/Campaigen.CLI.csproj
    ```

    This command will create the `campaigen.db` file (if it doesn't exist) and apply any pending migrations to set up the necessary tables based on the `AppDbContext`.

3.  **Adding New Migrations:**
    If you make changes to the Domain entities or `AppDbContext` configuration that require a schema change:
    - Navigate to the Infrastructure project directory (`src/Campaigen.Core.Infrastructure`).
    - Run the `add migration` command:
      ```bash
      dotnet ef migrations add YourMigrationName --startup-project ../Campaigen.CLI/Campaigen.CLI.csproj
      ```
      (Replace `YourMigrationName` with a descriptive name, e.g., `AddInfluencerContactInfo`).
    - Review the generated migration file in the `Migrations/` folder.
    - Apply the new migration using the `database update` command shown in step 2.

## Building the Application

To build the entire solution, navigate to the solution root directory and run:

```bash
dotnet build
```

## Running the Application (CLI)

You can run the CLI application using `dotnet run` from the solution root, specifying the CLI project:

```bash
dotnet run --project src/Campaigen.CLI/Campaigen.CLI.csproj -- [command] [subcommand] [options]
```

**Examples:**

- Show help:
  ```bash
  dotnet run --project src/Campaigen.CLI/Campaigen.CLI.csproj -- --help
  ```
- List influencers:
  ```bash
  dotnet run --project src/Campaigen.CLI/Campaigen.CLI.csproj -- influencer list
  ```
- Add a spend record:
  ```bash
  dotnet run --project src/Campaigen.CLI/Campaigen.CLI.csproj -- spend add --date "2024-03-31" --amount 150.75 --description "Software Subscription"
  ```

Refer to the [CLI documentation](cli.md) or use `--help` for detailed command usage.
