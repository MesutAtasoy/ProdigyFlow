using Microsoft.SemanticKernel;
using ProdigyFlow.AI.Factories;
using ProdigyFlow.AI.Plugins;
using ProdigyFlow.AI.Services;

Console.WriteLine("Starting ProdigyFlow AI...");

var prDiffFile = GetValidPrDiffPath(args);
if (prDiffFile is null) 
    return;

string prDiff = File.ReadAllText(prDiffFile);

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
var prioritizedResult = await kernel.InvokeAsync<List<string>>("TestPrioritization", "PrioritizeTests", new() { ["prDiff"] = prDiff, ["testList"] = string.Join(",", allTests) });
await fileService.WriteFileAsync("prioritized_tests.txt", prioritizedResult);


static string? GetValidPrDiffPath(string[] args)
{
    if (args.Length == 0)
    {
        Console.WriteLine("Error: PR diff file is required as argument.");
        return null;
    }

    string filePath = args[0];
    if (!File.Exists(filePath))
    {
        Console.WriteLine($"Error: PR diff file not found: {filePath}");
        return null;
    }

    return filePath;
}