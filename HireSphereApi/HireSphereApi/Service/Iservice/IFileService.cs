using HireSphereApi.core.DTO;
using HireSphereApi.core.entities;
using HireSphereApi.entities;

public interface IFileService
{
    Task<IEnumerable<FileDto>> GetAllFiles();
    Task<FileDto?> GetFileById(int id);
    Task<string> GeneratePresignedUrlToUpload(string fileName);
    Task<bool> DeleteFile(int fileId, int ownerId);

}
