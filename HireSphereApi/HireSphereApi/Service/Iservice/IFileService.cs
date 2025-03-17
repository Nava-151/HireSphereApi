using HireSphereApi.core.DTO;
using HireSphereApi.core.entities;
using HireSphereApi.entities;

public interface IFileService
{
    Task<IEnumerable<FileDto>> GetAllFiles();
    Task<FileDto?> GetFileById(int id);
    Task<FileDto> UploadFile(FileEntity file);
    Task<bool> DeleteFile(int fileId, int ownerId);
    Task<string> GetPresignedUrl(string fileKey);


}
