using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;

namespace ProdigyFlow.AI.Services;

public class AIService
{
    public IChatCompletionService _chatCompletionService;
    private Kernel _kernel;

    public async Task InitializeAsync()
    {
        IKernelBuilder kernelBuilder = Kernel.CreateBuilder();

        kernelBuilder.AddGoogleAIGeminiChatCompletion(
            modelId: "gemini-2.0-flash",
            apiKey: Environment.GetEnvironmentVariable("GOOGLE_GEMINI_API_KEY"),
            GoogleAIVersion.V1);
        
        _kernel = kernelBuilder.Build();
        _chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
        
        await Task.CompletedTask;
    }

    public async Task<string> SummarizePRAsync(string prDiff)
    {
        // Load prompt from file or inline
        ChatHistory history = [];
        history.AddUserMessage($"Summarize the following PR diff in a short summary:\n{prDiff}");

        try
        {
            var response = await _chatCompletionService.GetChatMessageContentAsync(
                history,
                kernel: _kernel
            );
        
        
            return response.Content;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<decimal> ComputeRiskScoreAsync(string prDiff)
    {
        ChatHistory history = [];
        history.AddUserMessage( $"Analyze the risk of this PR diff (0-100):\n{prDiff}");
        
        
        var result = await _chatCompletionService.GetChatMessageContentAsync(
            history,
            kernel: _kernel
        );

        if (decimal.TryParse(result.Content.Trim(), out var score))
            return score;

        return 0; // fallback
    }
    
    public async Task<decimal> DetectPipelineAnomalyAsync(double buildDuration, double testDuration, int failedTests)
    {
        // Build prompt for AI
        string prompt = $@"
You are an AI assistant that detects CI/CD pipeline anomalies.
Given the following metrics:
- Build duration: {buildDuration} seconds
- Test duration: {testDuration} seconds
- Failed tests: {failedTests}

Provide a numeric anomaly score between 0 and 1 (0 = normal, 1 = extreme anomaly) 
and a short explanation in JSON format like:
{{ ""score"": 0.7, ""reason"": ""2 failed tests and build duration longer than usual"" }}.
";

        ChatHistory history = [];
        history.AddUserMessage( prompt);
        
        // Call AI (Semantic Kernel or Gemini)
        var response = await _chatCompletionService.GetChatMessageContentAsync(
            history, new GeminiPromptExecutionSettings {MaxTokens = 500 }
        );

        // Example: parse JSON from AI response
        try
        {
            var json = System.Text.Json.JsonDocument.Parse(response.Content);
            decimal score = json.RootElement.GetProperty("score").GetDecimal();
            string reason = json.RootElement.GetProperty("reason").GetString() ?? "";

            Console.WriteLine($"Anomaly reason: {reason}");
            return Math.Clamp(score, 0, 1);
        }
        catch
        {
            // Fallback if AI fails or returns invalid JSON
            Console.WriteLine("AI failed to parse anomaly score, using heuristic fallback.");
            decimal fallbackScore = Math.Min((decimal)(buildDuration / 300), 1m) * 0.4m
                                    + Math.Min((decimal)(testDuration / 120), 1m) * 0.3m
                                    + Math.Min(failedTests, 10) / 10m * 0.3m;
            return Math.Clamp(fallbackScore, 0, 1);
        }
    }
}