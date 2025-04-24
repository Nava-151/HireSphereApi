using HireSphereApi.core.DTO;
using HireSphereApi.core.DTOs;
using HireSphereApi.entities;

public interface IFileService
{
    Task<IEnumerable<FileDto>> GetAllFiles();
    Task<FileDto?> GetFileByOwnnerId(int  ownerId);
    Task<IResult> AnalyzeResumeAsync(ResumeAnalyzeRequest request);

    Task<bool> DeleteFile( int ownerId);
    Task<FileDto?> AddFile(FilesPostModel file);



}
