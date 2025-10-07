using System.Text;
using System.Text.Json;
using Microsoft.SemanticKernel.ChatCompletion;

namespace ProdigyFlow.AI.Services;

public class RiskScoreService
{
    private readonly IChatCompletionService _chat;
    private readonly string _promptTemplate;

    public RiskScoreService(IChatCompletionService chat)
    {
        _chat = chat;
        _promptTemplate = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Prompts", "RiskScorePrompt.txt"));
    }

    public async Task<string> ComputeRiskScoreAsync(string prDiff)
    {
        var prompt = _promptTemplate
            .Replace("{PR_DIFF}", prDiff);

        // Get AI response
        var result = await _chat.GetChatMessageContentsAsync(prompt);

        var content = result.FirstOrDefault()?.Content;

        if (string.IsNullOrWhiteSpace(content))
            return "⚠️ No response received from AI model.";
        
        try
        {
            var jsonStart = content.IndexOf('{');
            var jsonEnd = content.LastIndexOf('}');
            var json = (jsonStart >= 0 && jsonEnd >= 0)
                ? content.Substring(jsonStart, jsonEnd - jsonStart + 1)
                : content;
            
            var riskAnalysis = JsonSerializer.Deserialize<RiskAnalysisResult>(json, new JsonSerializerOptions{ PropertyNameCaseInsensitive = true});

            if (riskAnalysis == null)
                return "⚠️ Failed to parse AI response into RiskAnalysisResult.";

            var sb = new StringBuilder();
            sb.AppendLine($"🔹 **Risk Score:** {riskAnalysis.RiskScore}/100");
            sb.AppendLine($"🔸 **Confidence:** {riskAnalysis.Confidence}%");

            if (riskAnalysis.Reasons != null && riskAnalysis.Reasons.Any())
            {
                sb.AppendLine("🧠 **Reasons:**");
                foreach (var reason in riskAnalysis.Reasons)
                    sb.AppendLine($"  • {reason}");
            }

            return sb.ToString();
        }
        catch(Exception ex)
        {
            return $"⚠️ Failed to parse AI response. Error: {ex.Message}\nRaw Response:\n{content}";
        }
    }
}

public class RiskAnalysisResult
{
    public decimal RiskScore { get; set; }
    public decimal Confidence { get; set; }
    public List<string> Reasons { get; set; }
}