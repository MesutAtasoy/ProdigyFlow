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
}