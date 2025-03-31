using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq; // Needed for Split and Skip
using System.Globalization; // For locale-aware decimal formatting

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
        /// <summary>
        /// Runs the CLI application with the specified arguments using the compiled DLL.
        /// </summary>
        /// <param name="args">The command-line arguments (pre-split) to pass to the CLI.</param>
        /// <param name="environmentVariables">Optional environment variables to set for the process.</param>
        /// <returns>A CliResult containing the exit code and captured output.</returns>
        public static async Task<CliResult> RunAsync(string[] args, Dictionary<string, string>? environmentVariables = null)
        {
            // Calculate paths relative to the current test execution directory
            var solutionRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
            var cliProjectDir = Path.Combine(solutionRoot, "src", "Campaigen.CLI");
            
            // Check for both Release and Debug configurations
            // First check the current test configuration (if we're running Release tests, check Release first)
            string? testConfiguration = null;
            
            // Try to determine whether we're running in Debug or Release from the path
            if (AppContext.BaseDirectory.Contains("Release"))
            {
                testConfiguration = "Release";
            }
            else if (AppContext.BaseDirectory.Contains("Debug"))
            {
                testConfiguration = "Debug";
            }
            
            // Try possible configurations, starting with the current test configuration if known
            string[] configurations = testConfiguration != null
                ? new[] { testConfiguration, testConfiguration == "Release" ? "Debug" : "Release" }
                : new[] { "Release", "Debug" };
                
            string cliDllPath = string.Empty;
            bool foundDll = false;
            
            foreach (var config in configurations)
            {
                var possiblePath = Path.Combine(cliProjectDir, "bin", config, "net8.0", "Campaigen.CLI.dll");
                if (File.Exists(possiblePath))
                {
                    cliDllPath = possiblePath;
                    foundDll = true;
                    break;
                }
            }

            if (!foundDll)
            {
                throw new FileNotFoundException($"CLI DLL not found in any configuration (checked Release and Debug). " +
                                                "Ensure the project is built before running tests.");
            }

            var processStartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = cliProjectDir
            };

            // Use ArgumentList for robustness
            processStartInfo.ArgumentList.Add(cliDllPath); // Add DLL path first

            // Process arguments to handle Windows/PowerShell specific issues
            foreach (var originalArg in args)
            {
                var arg = originalArg;
                
                // Check if argument is a numeric value and format it according to current culture
                if (decimal.TryParse(originalArg, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal decimalValue))
                {
                    // Use the current culture's decimal separator (comma for many European cultures)
                    arg = decimalValue.ToString(CultureInfo.CurrentCulture);
                }
                
                // Handle arguments with special characters
                if (arg.StartsWith("@"))
                {
                    // Remove @ prefix to avoid PowerShell response file issues
                    arg = arg.Substring(1);
                }
                
                processStartInfo.ArgumentList.Add(arg);
            }

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