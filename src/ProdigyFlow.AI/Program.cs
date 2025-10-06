using System.Diagnostics;
using System.Globalization;
using ProdigyFlow.AI.Services;

Console.WriteLine("Starting ProdigyFlow AI...");

if (args.Length < 2)
{
    Console.WriteLine("Usage: dotnet run --project src/ProdigyFlow.AI <Mode> <PRDiffFile>");
    Console.WriteLine("Modes: TestPrioritization | AnomalyDetection");
    return;
}

string mode = args[0];
string prDiffFile = args[1];

if (!File.Exists(prDiffFile))
{
    Console.WriteLine($"Error: PR diff file not found: {prDiffFile}");
    return;
}

string prDiff = File.ReadAllText(prDiffFile);

var aiService = new AIService();
await aiService.InitializeAsync();

switch (mode)
{
    case "TestPrioritization":
        // Summarize PR
        string summary = await aiService.SummarizePRAsync(prDiff);
        Console.WriteLine($"PR Summary: {summary}");
        var summaryFilePath = Path.Combine(AppContext.BaseDirectory, "ai_summary.txt");
        await File.WriteAllLinesAsync(summaryFilePath, new List<string> { summary });

        // Compute Risk Score
        decimal risk = await aiService.ComputeRiskScoreAsync(prDiff);
        Console.WriteLine($"Risk Score: {risk}");
        var riskFilePath = Path.Combine(AppContext.BaseDirectory, "ai_risk.txt");
        await File.WriteAllLinesAsync(riskFilePath, new List<string> { risk.ToString() });

        // Discover Tests
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

        Console.WriteLine("Discovered Tests:");
        Console.WriteLine(string.Join("\n", allTests));

        // Prioritize tests
        var testPrioritizationPrompt = Path.Combine(AppContext.BaseDirectory, "Prompts", "TestPrioritizationPrompt.txt");
        var testPrioritizationService = new TestPrioritizationService(aiService._chatCompletionService, testPrioritizationPrompt);
        var prioritizedTests = await testPrioritizationService.PrioritizeTestsAsync(prDiff, allTests);

        // Save prioritized tests to file
        var outputFile = Path.Combine(AppContext.BaseDirectory, "prioritized_tests.txt");
        await File.WriteAllLinesAsync(outputFile, prioritizedTests);
        Console.WriteLine($"Prioritized tests saved to: {outputFile}");
        Console.WriteLine("Tests to run:");
        Console.WriteLine(string.Join("\n", prioritizedTests));
        break;

    case "AnomalyDetection":
        // Read CI metrics from environment variables or input (set by GitHub Actions)
        if (!double.TryParse(Environment.GetEnvironmentVariable("BUILD_DURATION_SECONDS"), out double buildDuration))
            buildDuration = 0;

        if (!double.TryParse(Environment.GetEnvironmentVariable("TEST_DURATION_SECONDS"), out double testDuration))
            testDuration = 0;

        if (!int.TryParse(Environment.GetEnvironmentVariable("FAILED_TESTS_COUNT"), out int failedTests))
            failedTests = 0;

        Console.WriteLine($"Build Duration (s): {buildDuration}");
        Console.WriteLine($"Test Duration (s): {testDuration}");
        Console.WriteLine($"Failed Tests: {failedTests}");

        // Here you can call AI service to analyze anomaly, e.g., detect unusual durations
        var anomalyScore = await aiService.DetectPipelineAnomalyAsync(buildDuration, testDuration, failedTests);
        Console.WriteLine($"Pipeline Anomaly Score: {anomalyScore}");

        // Save anomaly metrics
        var anomalyFile = Path.Combine(AppContext.BaseDirectory, "ai_anomaly.txt");
        await File.WriteAllLinesAsync(anomalyFile, new List<string> { $"AnomalyScore: {anomalyScore}" });
        break;

    default:
        Console.WriteLine("Unknown mode. Use TestPrioritization or AnomalyDetection.");
        break;
}
