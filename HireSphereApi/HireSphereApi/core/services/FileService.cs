//using HireSphereApi.Data;
//using HireSphereApi.entities;
//using HireSphereApi.Service.Iservice;
//using AutoMapper;
//using Microsoft.EntityFrameworkCore;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using HireSphereApi.core.DTO;
//using Amazon.S3.Model;
//using Amazon.S3;
//using HireSphereApi.core.entities;

//public class FileService : IFileService
//{
//    private readonly DataContext _context;
//    private readonly IMapper _mapper;
//    private readonly IAmazonS3 _s3Client;


//    public FileService(DataContext context, IMapper mapper,IAmazonS3 s3Client)
//    {
//        _s3Client = s3Client;
//        _context = context;
//        _mapper = mapper;
//    }



//    public async Task<FileDto> UploadFile(FileEntity file)
//    {

//        file.CreatedAt = DateTime.UtcNow;
//        file.UpdatedAt = DateTime.UtcNow;

//        _context.Files.Add(file);
//        await _context.SaveChangesAsync();

//        return _mapper.Map<FileDto>(file);
//    }






//    public async Task<string> GetPresignedUrl(string fileKey)
//    {
//        if (string.IsNullOrEmpty(fileKey)) throw new ArgumentException("FileKey is required");

//        var request = new GetPreSignedUrlRequest
//        {
//            BucketName = "hiresphere",
//            Key = fileKey,
//            Expires = DateTime.UtcNow.AddMinutes(60) // לינק תקף ל-15 דקות
//        };

//        return _s3Client.GetPreSignedURL(request);
//    }
//}

using HireSphereApi.core.entities;
using Microsoft.EntityFrameworkCore;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System.Net.Http.Json;
using HireSphereApi.Data;
using HireSphereApi.core.DTO;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;


public class FileService : IFileService
{
    private readonly DataContext _context;  // Change to your actual DbContext name
    private readonly IAmazonS3 _s3Client;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly IMapper _mapper;


    //check if there is need to send access
    public FileService(DataContext context, IAmazonS3 s3Client, IConfiguration configuration, HttpClient httpClient, IMapper mapper)
    {
        _mapper = mapper;
        _context = context;
        _s3Client = s3Client;
        _configuration = configuration;
        _httpClient = httpClient;
    }

    public async Task<string> GeneratePresignedUrlToUpload([FromQuery] string fileName)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = "hiresphere",
            Key = fileName,
            Verb = HttpVerb.PUT,
            Expires = DateTime.UtcNow.AddMinutes(5),
            //ContentType = "application/pdf" // או סוג הקובץ המתאים
        };
        string url = _s3Client.GetPreSignedURL(request);
        return url ;
    }

    public async Task<Stream> DownloadFileAsync(string s3Key)
    {
        try
        {
            var request = new GetObjectRequest
            {
                BucketName = "hiresphere",
                Key = s3Key
            };

            using var response = await _s3Client.GetObjectAsync(request);
            Console.WriteLine(response);
            var memoryStream = new MemoryStream();
            await response.ResponseStream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }
        catch
        {
            return null;
        }
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

    
}
