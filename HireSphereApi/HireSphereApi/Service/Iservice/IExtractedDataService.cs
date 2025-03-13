using HireSphereApi.api.Models;
using HireSphereApi.core.DTO;
using HireSphereApi.core.entities;
public interface IExtractedDataService
{
    Task<IEnumerable<ExtractedDataDto>> GetAllData();
    Task<ExtractedDataDto?> GetDataById(int id);
    Task<ExtractedDataDto> CreateData(ExtractedDataPostModel dataModel);
    Task<bool> UpdateData(int id, ExtractedDataPostModel updatedData);
    Task<bool> DeleteData(int id);

    Task<List<ExtractedDataDto>> GetFilteredReports(AIResponse filterParams);
}
