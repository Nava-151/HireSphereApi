using HireSphereApi.core.entities;

namespace HireSphereApi.Service.Iservice
{
    public interface IAIService
    {
       Task<AIResponse> AnalyzeResumeAsync(string extractedText);

        Task<AIResponse> GetAIResponse(int aiId);

        Task<string> ChatWithAiAsync(string message);

    }
}
