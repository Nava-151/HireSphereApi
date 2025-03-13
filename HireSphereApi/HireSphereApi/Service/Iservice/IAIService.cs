using HireSphereApi.core.entities;

namespace HireSphereApi.Service.Iservice
{
    public interface IAIService
    {
        Task<ProjectAnalysisResult> ParseProjectDescription(string description);
    }
}
