using HireSphereApi.core.DTO;
using HireSphereApi.entities;

public interface IFileService
{
    Task<IEnumerable<FileDto>> GetAllFiles();
    Task<FileDto?> GetFileById(int id);
    Task<FileDto> UploadFile(FilesPostModel fileModel);
    Task<bool> DeleteFile(int id);
}
