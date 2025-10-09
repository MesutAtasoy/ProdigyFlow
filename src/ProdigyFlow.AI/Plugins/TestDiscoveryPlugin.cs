using System.ComponentModel;
using System.Diagnostics;
using Microsoft.SemanticKernel;

namespace ProdigyFlow.AI.Plugins;

public class TestDiscoveryPlugin
{
    [KernelFunction, Description("Discovers all unit tests in the solution")]
    public Task<List<string>> DiscoverTests()
    {
        var testProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "test ProdigyFlow.sln --list-tests --no-build --configuration Release",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        testProcess.Start();
        var output = testProcess.StandardOutput.ReadToEnd();
        testProcess.WaitForExit();

        var tests = output
            .Split('\n')
            .Select(t => t.Trim())
            .Where(t => !string.IsNullOrWhiteSpace(t) && !t.StartsWith("The following", StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Task.FromResult(tests);
    }
}