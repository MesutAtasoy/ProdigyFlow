using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace ProdigyFlow.AI.Plugins;


public class TestPrioritizationPlugin
{
    private readonly Kernel _kernel;
    private readonly string _promptTemplate;

    public TestPrioritizationPlugin(Kernel kernel)
    {
        _kernel = kernel;
        _promptTemplate = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Prompts", "TestPrioritizationPrompt.txt"));
    }

    [KernelFunction, Description("Prioritizes a list of tests based on PR diff")]
    public async Task<List<string>> PrioritizeTests(
        [Description("The PR diff")] string prDiff,
        [Description("Comma-separated list of test names")] string testList)
    {
        var tests = testList.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(t => t.Trim())
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .ToList();

        var prompt = _promptTemplate
            .Replace("{PR_DIFF}", prDiff)
            .Replace("{TEST_LIST}", string.Join(", ", tests));

        var result = await _kernel.InvokePromptAsync(prompt);
        var raw = result.ToString() ?? "";

        // Extract valid test names only
        var prioritized = raw
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