using HireSphereApi.core.entities;

namespace HireSphereApi.Service.Iservice
{
    public interface IAIService
    {
        Task<ExtractedDataEntity?> AnalyzeAndStoreDataAsync(int candidateId, string fileKey);

    }
}
