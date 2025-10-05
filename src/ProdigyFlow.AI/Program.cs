using ProdigyFlow.AI.Services;

Console.WriteLine("Starting ProdigyFlow AI...");

var aiService = new AIService();
await aiService.InitializeAsync();

string dummyDiff = File.ReadAllText("dummy_diff.txt");

// Summarize PR
string summary = await aiService.SummarizePRAsync(dummyDiff);
Console.WriteLine($"PR Summary: {summary}");

// Compute Risk Score
decimal risk = await aiService.ComputeRiskScoreAsync(dummyDiff);
Console.WriteLine($"Risk Score: {risk}");

var allTests = new List<string>
{
    "ProductService_AddProduct_Test",
    "ProductService_GetAllProducts_Test",
    "CategoryService_AddCategory_Test",
    "CategoryService_GetAllCategories_Test"
};

// Prioritize tests
var testPrioritizationService = new TestPrioritizationService(aiService._chatCompletionService, "Prompts/TestPrioritizationPrompt.txt");
var prioritizedTests = await testPrioritizationService.PrioritizeTestsAsync(dummyDiff, allTests);

Console.WriteLine("Prioritized Tests:");
Console.WriteLine(string.Join("\n", prioritizedTests));