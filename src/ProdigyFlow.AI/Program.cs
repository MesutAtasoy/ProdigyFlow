using Microsoft.SemanticKernel;
using ProdigyFlow.AI.Factories;
using ProdigyFlow.AI.Plugins;
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

Console.WriteLine($"prDiff: {prDiff}");


IKernelFactory kernelFactory = new DefaultKernelFactory(); // or inject via DI
var kernel = kernelFactory.CreateKernel();

kernel.ImportPluginFromObject(new PRAnalysisPlugin(kernel), "PRAnalysis");
kernel.ImportPluginFromObject(new TestDiscoveryPlugin(), "TestDiscovery");
kernel.ImportPluginFromObject(new TestPrioritizationPlugin(kernel), "TestPrioritization");


var fileService = new FileService();

// Summarize PR
var summaryResult = await kernel.InvokeAsync<string>("PRAnalysis", "SummarizePR", new() { ["prDiff"] = prDiff });
await fileService.WriteFileAsync("ai_summary.txt", summaryResult);

var riskResult = await kernel.InvokeAsync<string>("PRAnalysis", "ComputeRiskScore", new() { ["prDiff"] = prDiff });
await fileService.WriteFileAsync("ai_risk.txt", riskResult);

var allTests = await kernel.InvokeAsync<List<string>>("TestDiscovery", "DiscoverTests");
Console.WriteLine($"✅ Discovered {allTests.Count} tests.");

// Prioritize tests
var prioritizedResult = await kernel.InvokeAsync<List<string>>("TestPrioritization", "PrioritizeTests", new() { ["prDiff"] = prDiff, ["testList"] = string.Join(",", allTests) });

// Save prioritized tests to output file for GitHub Actions
await fileService.WriteFileAsync("prioritized_tests.txt", prioritizedResult);