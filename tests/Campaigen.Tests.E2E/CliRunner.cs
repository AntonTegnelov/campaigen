using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Campaigen.Tests.E2E
{
    /// <summary>
    /// Represents the result of a CLI command execution.
    /// </summary>
    public class CliResult
    {
        public int ExitCode { get; }
        public string StandardOutput { get; }
        public string StandardError { get; }
        public bool IsSuccess => ExitCode == 0;

        public CliResult(int exitCode, string standardOutput, string standardError)
        {
            ExitCode = exitCode;
            StandardOutput = standardOutput;
            StandardError = standardError;
        }
    }

    /// <summary>
    /// Helper class to run the Campaigen.CLI application and capture its output.
    /// </summary>
    public static class CliRunner
    {
        // Path to the CLI executable relative to the test project output directory
        // Adjust this path based on your build output structure.
        // Common structure: E2E tests are in tests/..., CLI output is in src/.../bin/...
        // Example: ..\..\..\..\src\Campaigen.CLI\bin\Debug\net8.0\Campaigen.CLI.exe (Windows)
        // Example: ../../../../src/Campaigen.CLI/bin/Debug/net8.0/Campaigen.CLI (Linux/macOS)
        private static readonly string CliExecutablePath = Path.Combine(
            AppContext.BaseDirectory, // Start from tests/Project/bin/Debug/net8.0
            "..", // bin/Debug
            "..", // bin
            "..", // tests/Project
            "..", // tests
            "..", // Workspace Root
            "src", "Campaigen.CLI", "bin",
#if DEBUG
            "Debug",
#else
            "Release",
#endif
            "net8.0", // Adjust target framework if needed
            "Campaigen.CLI.exe" // Change to "Campaigen.CLI" for Linux/macOS
        );


        /// <summary>
        /// Runs the CLI application with the specified arguments.
        /// </summary>
        /// <param name="arguments">The command-line arguments to pass to the CLI.</param>
        /// <param name="environmentVariables">Optional environment variables to set for the process.</param>
        /// <returns>A CliResult containing the exit code and captured output.</returns>
        public static async Task<CliResult> RunAsync(string arguments, Dictionary<string, string>? environmentVariables = null)
        {
            if (!File.Exists(CliExecutablePath))
            {
                throw new FileNotFoundException($"CLI executable not found at expected path: {Path.GetFullPath(CliExecutablePath)}. " +
                                                "Ensure the Campaigen.CLI project is built and the path in CliRunner.cs is correct.");
            }

            var processStartInfo = new ProcessStartInfo
            {
                FileName = CliExecutablePath,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = Path.GetDirectoryName(CliExecutablePath) ?? AppContext.BaseDirectory // Run from CLI output dir
            };

            // Set environment variables if provided
            if (environmentVariables != null)
            {
                foreach (var kvp in environmentVariables)
                {
                    processStartInfo.EnvironmentVariables[kvp.Key] = kvp.Value;
                    // Console.WriteLine($"Setting ENV: {kvp.Key}={kvp.Value}"); // Debugging line
                }
            }

            using var process = new Process { StartInfo = processStartInfo };

            var outputBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();

            var outputTaskCompletionSource = new TaskCompletionSource<bool>();
            var errorTaskCompletionSource = new TaskCompletionSource<bool>();

            process.OutputDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    outputBuilder.AppendLine(e.Data);
                }
                else
                {
                    outputTaskCompletionSource.TrySetResult(true);
                }
            };

            process.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    errorBuilder.AppendLine(e.Data);
                }
                else
                {
                    errorTaskCompletionSource.TrySetResult(true);
                }
            };

            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            // Wait for the process to exit and for the output/error streams to be closed
            await Task.WhenAll(
                 Task.Run(() => process.WaitForExit()), // This needs to run on a separate thread
                 outputTaskCompletionSource.Task,
                 errorTaskCompletionSource.Task
             );


            return new CliResult(process.ExitCode, outputBuilder.ToString().TrimEnd(), errorBuilder.ToString().TrimEnd());
        }
    }
} 