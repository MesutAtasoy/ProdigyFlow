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

// Unit tests
var unitTestService = new UnitTestService();
var allTests = unitTestService.GetDiscoveredTests();;

Console.WriteLine("Discovered Tests:");
Console.WriteLine(string.Join("\n", allTests));

// Prioritize tests
var testPrioritizationService = new TestPrioritizationService(aiService._chatCompletionService);
var prioritizedTests = await testPrioritizationService.PrioritizeTestsAsync(prDiff, allTests);

// Save prioritized tests to output file for GitHub Actions
await fileService.WriteFileAsync("prioritized_tests.txt", prioritizedTests);