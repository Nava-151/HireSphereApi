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

public class FileService : IFileService
{
    private readonly DataContext _context;  // Change to your actual DbContext name
    private readonly IAmazonS3 _s3Client;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly IMapper _mapper;



    public FileService(DataContext context, IAmazonS3 s3Client, IConfiguration configuration, HttpClient httpClient, IMapper mapper)
    {
        _mapper = mapper;
        _context = context;
        _s3Client = s3Client;
        _configuration = configuration;
        _httpClient = httpClient;
    }

    public async Task<FileEntity?> UploadFileAsync(Stream fileStream, string fileName, long fileSize, int candidateId)
    {
        string s3Key = $"uploads/{candidateId}/{Guid.NewGuid()}_{fileName}";

        // 1️⃣ Upload file to S3
        var uploadRequest = new TransferUtilityUploadRequest
        {
            InputStream = fileStream,
            BucketName = _configuration["AWS:BucketName"], // Ensure this is correctly set in appsettings.json
            Key = s3Key
        };
        var fileTransferUtility = new TransferUtility(_s3Client);
        await fileTransferUtility.UploadAsync(uploadRequest);

        // 2️⃣ Save file details in `FileEntity`
        var fileEntity = new FileEntity
        {
            FileName = fileName,
            FileType = Path.GetExtension(fileName),
            Size = fileSize,
            S3Key = s3Key,
            OwnerId = candidateId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Files.Add(fileEntity);
        await _context.SaveChangesAsync();

        // 3️⃣ Call AI and store extracted data
        await AnalyzeAndStoreDataAsync(candidateId, s3Key);

        return fileEntity;
    }

    private async Task AnalyzeAndStoreDataAsync(int candidateId, string fileKey)
    {
        // 4️⃣ Generate a pre-signed S3 URL for AI processing
        string signedUrl = GeneratePresignedUrl(fileKey);
        if (string.IsNullOrEmpty(signedUrl))
            return;

        // 5️⃣ Send the file to AI for analysis
        var aiResponse = await SendS3UrlToAI(signedUrl);
        if (aiResponse == null)
            return;

        // 6️⃣ Save AI response in `AIResponse`
        _context.AIResponses.Add(aiResponse);
        await _context.SaveChangesAsync();

        // 7️⃣ Link extracted data with candidate and file
        var extractedData = new ExtractedDataEntity
        {
            CandidateId = candidateId,
            FileKey = fileKey,
            IdResponse = aiResponse.Id, // Foreign key to AI response
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.ExtractedData.Add(extractedData);
        await _context.SaveChangesAsync();
    }

    private string GeneratePresignedUrl(string s3Key)
    {
        try
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _configuration["AWS:BucketName"],
                Key = s3Key,
                Expires = DateTime.UtcNow.AddMinutes(30) // Expiration time
            };

            return _s3Client.GetPreSignedURL(request);
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

    private async Task<AIResponse?> SendS3UrlToAI(string fileUrl)
    {
        var requestBody = new { fileUrl = fileUrl }; // Modify if your AI API requires different input
        var response = await _httpClient.PostAsJsonAsync("https://your-ai-api.com/analyze", requestBody);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<AIResponse>();
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

    public Task<FileDto> UploadFileAsync(FileEntity file)
    {
        throw new NotImplementedException();
    }

    Task<string> IFileService.GeneratePresignedUrl(string fileKey)
    {
        throw new NotImplementedException();
    }
}
