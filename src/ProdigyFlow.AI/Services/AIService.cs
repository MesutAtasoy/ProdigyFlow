using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;

namespace ProdigyFlow.AI.Services;

public class AIService
{
    private IChatCompletionService _chatCompletionService;
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
}