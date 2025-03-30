# CLI (`Campaigen.CLI`)

This project provides the command-line interface for interacting with the Campaigen application.

## Key Responsibilities

- Parse command-line arguments and options.
- Define the structure of commands and subcommands.
- Invoke the appropriate Application layer services based on user input.
- Display output (results or errors) to the user.
- Configure and initialize the application host (dependency injection, configuration, logging).

## Structure

- **`Program.cs`**: The main entry point. Configures the .NET Generic Host, sets up Dependency Injection (registers services, repositories, DbContext), configures logging and configuration sources (`appsettings.json`), builds the command structure using `System.CommandLine`, and invokes the command parser.
- **`Commands/`**: Contains classes defining the CLI commands and their handlers.
  - `InfluencerCommands.cs`: Defines the `influencer` command with `add` and `list` subcommands and their corresponding handlers (`AddInfluencerHandler`, `ListInfluencerHandler`). Handlers receive injected `IInfluencerService`.
  - `SpendCommands.cs`: Defines the `spend` command with `add` and `list` subcommands and their corresponding handlers (`AddSpendHandler`, `ListSpendHandler`). Handlers receive injected `ISpendTrackingService`.
- **`appsettings.json`**: Configuration file, primarily used for the database connection string (`ConnectionStrings:DefaultConnection`).

## Key Technologies

- **`System.CommandLine`**: Library used for parsing arguments and defining the command structure.
- **`Microsoft.Extensions.Hosting`**: Used for application host setup (DI, configuration, logging).
- **`Microsoft.Extensions.DependencyInjection`**: Used for managing application dependencies.
- **`Microsoft.Extensions.Configuration`**: Used for reading configuration files.

## Usage

(This section should be expanded with examples once the commands are more finalized)

Example (conceptual):

```bash
dotnet run --project src/Campaigen.CLI/Campaigen.CLI.csproj -- influencer add --name "Jane Doe" --handle "@janedoe" --platform "Instagram"

dotnet run --project src/Campaigen.CLI/Campaigen.CLI.csproj -- spend list
```

## Design Considerations

- **Dependency Injection:** Command handlers correctly receive dependencies (Application services) via constructor injection, facilitated by `System.CommandLine.Hosting`.
- **Output Formatting:** Current output within handlers is basic `Console.WriteLine`. Could be enhanced with more structured output (e.g., tables using libraries like Spectre.Console) for better readability.
- **Error Handling:** Command handlers should gracefully handle potential errors returned from the Application services (e.g., null DTOs, exceptions) and provide user-friendly error messages.
- **Command Structure:** The current `add`/`list` structure is logical. As features grow, consider how to organize more complex commands or options.
