using System.ComponentModel;
using System.Text;
using System.Text.Json;
using Microsoft.SemanticKernel;

namespace ProdigyFlow.AI.Plugins;

public class PRAnalysisPlugin
{
    private readonly Kernel _kernel;
    private readonly string _riskPrompt;

    public PRAnalysisPlugin(Kernel kernel)
    {
        _kernel = kernel;
        _riskPrompt = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Prompts", "RiskScorePrompt.txt"));
    }

    [KernelFunction, Description("Summarizes a PR diff")]
    public async Task<string> SummarizePR([Description("The PR diff content")] string prDiff)
    {
        var prompt = $"Summarize the following PR diff in a short, clear summary:\n{prDiff}";
        var result = await _kernel.InvokePromptAsync(prompt);
        return result.ToString() ?? "No summary available.";
    }

    [KernelFunction, Description("Computes a risk score from a PR diff")]
    public async Task<string> ComputeRiskScore([Description("The PR diff content")] string prDiff)
    {
        var prompt = _riskPrompt.Replace("{PR_DIFF}", prDiff);
        var result = await _kernel.InvokePromptAsync(prompt);
        var content = result.ToString() ?? "";

        try
        {
            var jsonStart = content.IndexOf('{');
            var jsonEnd = content.LastIndexOf('}');
            var json = (jsonStart >= 0 && jsonEnd >= 0)
                ? content.Substring(jsonStart, jsonEnd - jsonStart + 1)
                : content;

            var risk = JsonSerializer.Deserialize<RiskAnalysisResult>(
                json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (risk == null)
                return "⚠️ Failed to parse risk analysis.";

            var sb = new StringBuilder();
            sb.AppendLine($"🔹 **Risk Score:** {risk.RiskScore}/100");
            sb.AppendLine($"🔸 **Confidence:** {risk.Confidence}%");
            if (risk.Reasons?.Any() == true)
            {
                sb.AppendLine("🧠 **Reasons:**");
                foreach (var r in risk.Reasons) sb.AppendLine($"  • {r}");
            }
            return sb.ToString();
        }
        catch (Exception ex)
        {
            return $"⚠️ Parse error: {ex.Message}\nRaw: {content}";
        }
    }
}

public class RiskAnalysisResult
{
    public decimal RiskScore { get; set; }
    public decimal Confidence { get; set; }
    public List<string>? Reasons { get; set; }
}