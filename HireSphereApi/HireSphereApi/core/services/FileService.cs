using HireSphereApi.Data;
using HireSphereApi.entities;
using HireSphereApi.Service.Iservice;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using HireSphereApi.core.DTO;
using Amazon.S3.Model;
using Amazon.S3;
using HireSphereApi.core.entities;

public class FileService : IFileService
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    private readonly IAmazonS3 _s3Client;


    public FileService(DataContext context, IMapper mapper,IAmazonS3 s3Client)
    {
        _s3Client = s3Client;
        _context = context;
        _mapper = mapper;
    }


    public async Task<IEnumerable<FileDto>> GetAllFiles()
    {
        var files = await _context.Files.ToListAsync();
        return _mapper.Map<IEnumerable<FileDto>>(files);
    }

    public async Task<FileDto?> GetFileById(int id)
    {
        var file = await _context.Files.FindAsync(id);
        return file != null ? _mapper.Map<FileDto>(file) : null;
    }


    public async Task<FileDto> UploadFile(FileEntity file)
    {

        file.CreatedAt = DateTime.UtcNow;
        file.UpdatedAt = DateTime.UtcNow;

        _context.Files.Add(file);
        await _context.SaveChangesAsync();

        return _mapper.Map<FileDto>(file);
    }



    public async Task<bool> DeleteFile(int fileId, int ownerId)
    {
        var file = await _context.Files
        .FirstOrDefaultAsync(f => f.Id == fileId && f.OwnerId == ownerId);

        if (file == null) return false; // קובץ לא נמצא או שייך למשתמש אחר

        file.IsDeleted = true;
        file.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }



    public async Task<string> GetPresignedUrl(string fileKey)
    {
        if (string.IsNullOrEmpty(fileKey)) throw new ArgumentException("FileKey is required");

        var request = new GetPreSignedUrlRequest
        {
            BucketName = "hiresphere",
            Key = fileKey,
            Expires = DateTime.UtcNow.AddMinutes(60) // לינק תקף ל-15 דקות
        };

        return _s3Client.GetPreSignedURL(request);
    }
}

