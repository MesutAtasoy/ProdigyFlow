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