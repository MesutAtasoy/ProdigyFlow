using Microsoft.SemanticKernel.ChatCompletion;

namespace ProdigyFlow.AI.Services;

public class TestPrioritizationService
{
    private readonly IChatCompletionService _chat;
    private readonly string _promptTemplate;

    public TestPrioritizationService(IChatCompletionService chat, string promptPath)
    {
        _chat = chat;
        _promptTemplate = File.ReadAllText(promptPath);
    }

    public async Task<List<string>> PrioritizeTestsAsync(string prDiff, List<string> testList)
    {
        var prompt = _promptTemplate
            .Replace("{PR_DIFF}", prDiff)
            .Replace("{TEST_LIST}", string.Join(", ", testList));

        var result = await _chat.GetChatMessageContentsAsync(prompt);

        var prioritized = result.FirstOrDefault()?.Content?.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(t => t.Trim())
            .ToList();

        return prioritized;
    }
}