using Microsoft.SemanticKernel;

namespace ProdigyFlow.AI.Factories;

public interface IKernelFactory
{
    Kernel CreateKernel();
}