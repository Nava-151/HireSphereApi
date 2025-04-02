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
using HireSphereApi.core.services;
using HireSphereApi.Service.Iservice;
using HireSphereApi.entities;
using iText.StyledXmlParser.Jsoup.Nodes;


public class FileService : IFileService
{
    private readonly DataContext _context;  // Change to your actual DbContext name
    private readonly IAmazonS3 _s3Client;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly IMapper _mapper;
    private readonly IS3Service _s3Service;
    private readonly IAIService _aiService;
    private readonly TextExtractionService _textService;

    public FileService(IS3Service s3Service, IAIService aiService, TextExtractionService textService, DataContext context, IAmazonS3 s3Client, IConfiguration configuration, HttpClient httpClient, IMapper mapper)
    {
        _mapper = mapper;
        _context = context;
        _s3Client = s3Client;
        _configuration = configuration;
        _httpClient = httpClient;
        _s3Service = s3Service;
        _aiService = aiService;
        _textService = textService;
    }


    public async Task<IResult> AnalyzeResumeAsync(ResumeAnalyzeRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.S3Key))
                return Results.BadRequest("Invalid S3 key.");

            // Generate S3 URL
            var fileUrl = await _s3Service.GeneratePresignedUrlToDownload(request.S3Key);
            Console.WriteLine($"Generated S3 URL: {fileUrl}");

            if (fileUrl == null)
                return Results.BadRequest("File not found in S3.");

            // Fetch file from S3
            var response = await _httpClient.GetAsync(fileUrl);
            Console.WriteLine($"HTTP Response Status: {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error fetching file: {errorMessage}");
                return Results.Problem($"Failed to fetch the file. Status Code: {response.StatusCode}, Error: {errorMessage}");
            }

            // Read file stream
            using var fileStream = await response.Content.ReadAsStreamAsync();
            using var memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            Console.WriteLine($"File stream length: {memoryStream.Length}");

            // Extract text
            string extractedText = _textService.ExtractTextFromFile(memoryStream, request.S3Key);
            Console.WriteLine($"Extracted text: {extractedText}");

            if (string.IsNullOrWhiteSpace(extractedText))
                return Results.BadRequest("Failed to extract text from file.");

            // Analyze text with AI
            var aiResponse = await _aiService.AnalyzeResumeAsync(extractedText);
            if (aiResponse == null)
            {
                return Results.BadRequest("Error: Could not analyze resume.");
            }

            // Save AI response
            _context.AIResponses.Add(aiResponse);
            await _context.SaveChangesAsync();

            var extractedData = new ExtractedDataEntity
            {
                CandidateId = request.UserId,
                FileKey = request.S3Key,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IdResponse = aiResponse.Id
            };

            _context.ExtractedData.Add(extractedData);
            await _context.SaveChangesAsync();

            return Results.Ok(new { extractedData.Id });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return Results.Problem($"Unexpected error: {ex.Message}");
        }
    }



    //no use of it- maby rhe logicak isnt goof

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

    //also in it 
    public async Task<bool> DeleteFile( int ownerId)
    {
        var file = await _context.Files
        .FirstOrDefaultAsync(f => f.OwnerId == ownerId&& f.IsDeleted==true );

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

    public async Task<FileDto?> GetFileByOwnnerId(int ownerId)
    {
        var file = await _context.Files.FirstOrDefaultAsync(u => u.OwnerId == ownerId&&u.IsDeleted==true); 
            return file != null ? _mapper.Map<FileDto>(file) : null;
    }


    public async Task<FileDto?> AddFile(FilesPostModel file)
    {

        var fileEntity = _mapper.Map<FileEntity>(file);
        _context.Files.Add(fileEntity);
        await _context.SaveChangesAsync();
        return _mapper.Map<FileDto>(fileEntity);
    }

}
