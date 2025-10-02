using HireSphereApi.api.Models;
using HireSphereApi.core.DTO;
using HireSphereApi.core.DTOs;
using HireSphereApi.core.entities;
public interface IExtractedDataService
{
    Task<IEnumerable<ExtractedDataDto>> GetAllData();
    Task<ExtractedDataDto?> GetDataById(int id);
    Task<ExtractedDataDto> CreateData(ExtractedDataPostModel dataModel);
    Task<ExtractedDataDto> UpdateData(int id, ExtractedDataPostModel updatedData);
    Task<bool> DeleteData(int id);
    Task<IEnumerable<ExtractedDataDto>> GetFilteredReports(AiResponseDto filterParams);
    Task<ExtractedDataDto> AddMark(decimal mark, int userId);

}
