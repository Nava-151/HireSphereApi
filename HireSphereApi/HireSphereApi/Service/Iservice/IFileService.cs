using HireSphereApi.core.DTO;
using HireSphereApi.core.entities;
using HireSphereApi.entities;

public interface IFileService
{
    Task<IEnumerable<FileDto>> GetAllFiles();
    Task<FileDto?> GetFileById(int id);
    Task<FileEntity> UploadFileAsync(Stream fileStream, string fileName, long fileSize, int candidateId);
    Task<bool> DeleteFile(int fileId, int ownerId);
    Task<string> GeneratePresignedUrl(string fileKey);


}
