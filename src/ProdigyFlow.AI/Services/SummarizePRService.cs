using Microsoft.SemanticKernel.ChatCompletion;

namespace ProdigyFlow.AI.Services;

public class SummarizePRService
{
    private readonly IChatCompletionService _chatCompletionService;

    public SummarizePRService(IChatCompletionService chat)
    {
        _chatCompletionService = chat;
    }
    
    public async Task<string> SummarizeAsync(string prDiff)
    {
        ChatHistory history = [];
        history.AddUserMessage($"Summarize the following PR diff in a short summary:\n{prDiff}");

        try
        {
            var response = await _chatCompletionService.GetChatMessageContentAsync(history);
        
            return response.Content;
        }
        catch (Exception e)
        {
            return "No available summary!";
        }
    }
}