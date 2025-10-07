using System.Diagnostics;
using ProdigyFlow.AI.Services;

Console.WriteLine("Starting ProdigyFlow AI...");


if (args.Length == 0)
{
    Console.WriteLine("Error: PR diff file is required as argument.");
    return;
}

string prDiffFile = args[0];
if (!File.Exists(prDiffFile))
{
    Console.WriteLine($"Error: PR diff file not found: {prDiffFile}");
    return;
}

string prDiff = File.ReadAllText(prDiffFile);

var aiService = new AIService();
var fileService = new FileService();

await aiService.InitializeAsync();


// Summarize PR
var summarizePRService = new SummarizePRService(aiService._chatCompletionService);
string summary = await summarizePRService.SummarizeAsync(prDiff);
Console.WriteLine($"PR Summary: {summary}");
await fileService.WriteFileAsync("ai_summary.txt", summary);

// Compute Risk Score
var riskScoreService = new RiskScoreService(aiService._chatCompletionService);
var risk = await riskScoreService.ComputeRiskScoreAsync(prDiff);
Console.WriteLine($"Risk: {risk}");
await fileService.WriteFileAsync("ai_risk.txt", risk);

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

// Parse test names (skip headers)
var allTests = output
    .Split('\n')
    .Select(t => t.Trim())
    .Where(t => !string.IsNullOrWhiteSpace(t) && !t.StartsWith("The following"))
    .ToList();

Console.WriteLine("Discovered Tests:");
Console.WriteLine(string.Join("\n", allTests));

// Prioritize tests
var testPrioritizationService = new TestPrioritizationService(aiService._chatCompletionService);
var prioritizedTests = await testPrioritizationService.PrioritizeTestsAsync(prDiff, allTests);

// Save prioritized tests to output file for GitHub Actions
await fileService.WriteFileAsync("prioritized_tests.txt", prioritizedTests);