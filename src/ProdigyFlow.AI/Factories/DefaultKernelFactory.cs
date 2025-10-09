using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Google;

namespace ProdigyFlow.AI.Factories;

public class DefaultKernelFactory : IKernelFactory
{
    public Kernel CreateKernel()
    {
        IKernelBuilder kernelBuilder = Kernel.CreateBuilder();

        kernelBuilder.AddGoogleAIGeminiChatCompletion(
            modelId: "gemini-2.0-flash",
            apiKey: Environment.GetEnvironmentVariable("GOOGLE_GEMINI_API_KEY"),
            GoogleAIVersion.V1);
        
        return kernelBuilder.Build();
    }
}