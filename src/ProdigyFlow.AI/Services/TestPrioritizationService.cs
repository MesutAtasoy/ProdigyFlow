using Microsoft.SemanticKernel.ChatCompletion;

namespace ProdigyFlow.AI.Services;

public class TestPrioritizationService
{
    private readonly IChatCompletionService _chat;
    private readonly string _promptTemplate;

    public TestPrioritizationService(IChatCompletionService chat)
    {
        _chat = chat;
        _promptTemplate = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Prompts", "TestPrioritizationPrompt.txt"));
    }

    public async Task<List<string>> PrioritizeTestsAsync(string prDiff, List<string> testList)
    {
        var prompt = _promptTemplate
            .Replace("{PR_DIFF}", prDiff)
            .Replace("{TEST_LIST}", string.Join(", ", testList));

        // Get AI response
        var result = await _chat.GetChatMessageContentsAsync(prompt);

        var rawOutput = result.FirstOrDefault()?.Content ?? string.Empty;

        // Sanitize output: keep only valid test names
        var prioritized = rawOutput
            .Split(new[] { ',', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(t => t.Trim())
            .Where(t =>
                    !string.IsNullOrWhiteSpace(t) &&
                    !t.Contains(' ') && // filter out any stray text
                    t.Length > 3 &&
                    testList.Contains(t) // must exist in actual test list
            )
            .Distinct()
            .ToList();

        return prioritized;
    }
}