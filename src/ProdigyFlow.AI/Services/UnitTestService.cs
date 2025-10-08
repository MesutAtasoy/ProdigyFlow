using System.Diagnostics;

namespace ProdigyFlow.AI.Services;

public class UnitTestService
{
    public List<string> GetDiscoveredTests()
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
        
        var allTests = output
            .Split('\n')
            .Select(t => t.Trim())
            .Where(t => !string.IsNullOrWhiteSpace(t) && !t.StartsWith("The following"))
            .ToList();

        return allTests;
    }
}