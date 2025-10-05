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
await aiService.InitializeAsync();

// Summarize PR
string summary = await aiService.SummarizePRAsync(prDiff);
Console.WriteLine($"PR Summary: {summary}");

// Compute Risk Score
decimal risk = await aiService.ComputeRiskScoreAsync(prDiff);
Console.WriteLine($"Risk Score: {risk}");

var allTests = new List<string>
{
    "ProductService_AddProduct_Test",
    "ProductService_GetAllProducts_Test",
    "CategoryService_AddCategory_Test",
    "CategoryService_GetAllCategories_Test"
};

// Prioritize tests
var promptPath = Path.Combine(AppContext.BaseDirectory, "Prompts", "TestPrioritizationPrompt.txt");

var testPrioritizationService = new TestPrioritizationService(aiService._chatCompletionService, promptPath);
var prioritizedTests = await testPrioritizationService.PrioritizeTestsAsync(prDiff, allTests);

// Save prioritized tests to output file for GitHub Actions
var outputFile = Path.Combine(AppContext.BaseDirectory, "prioritized_tests.txt");
await File.WriteAllLinesAsync(outputFile, prioritizedTests);

Console.WriteLine($"Prioritized tests saved to: {outputFile}");
Console.WriteLine("Tests to run:");
Console.WriteLine(string.Join("\n", prioritizedTests));