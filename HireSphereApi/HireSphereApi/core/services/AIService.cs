using HireSphereApi.core.entities;
using HireSphereApi.entities;

namespace HireSphereApi.core.Services
{
    using HireSphereApi.core.entities;
    using Microsoft.EntityFrameworkCore;
    using Amazon.S3;
    using Amazon.S3.Model;
    using System.Net.Http.Json;
    using HireSphereApi.Data;
    using HireSphereApi.Service.Iservice;

    public class AIService:IAIService
    {
        private readonly DataContext _context;
        private readonly HttpClient _httpClient;
        private readonly IAmazonS3 _s3Client;
        private readonly IConfiguration _configuration;

        public AIService(DataContext context, HttpClient httpClient, IAmazonS3 s3Client, IConfiguration configuration)
        {
            _context = context;
            _httpClient = httpClient;
            _s3Client = s3Client;
            _configuration = configuration;
        }

        public async Task<ExtractedDataEntity?> AnalyzeAndStoreDataAsync(int candidateId, string fileKey)
        {
            // 1️⃣ חיפוש הקובץ בטבלת `Files`
            var file = await _context.Files.FirstOrDefaultAsync(f => f.S3Key == fileKey);
            if (file == null || file.IsDeleted)
                return null;

            // 2️⃣ יצירת URL חתום להורדה מה-S3
            string signedUrl = GeneratePresignedUrl(file.S3Key);
            if (string.IsNullOrEmpty(signedUrl))
                return null;

            // 3️⃣ שליחת הקובץ ל-AI
            var aiResponse = await SendS3UrlToAI(signedUrl);
            if (aiResponse == null)
                return null;

            // 4️⃣ שמירת תשובת ה-AI בטבלת `AIResponse`
            _context.AIResponses.Add(aiResponse);
            await _context.SaveChangesAsync();

            // 5️⃣ יצירת רשומה חדשה ב- `ExtractedDataEntity` עם המפתח הזר
            var extractedData = new ExtractedDataEntity
            {
                CandidateId = candidateId,
                FileKey = fileKey,
                IdResponse = aiResponse.Id, // מפתח זר לתשובת ה-AI
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.ExtractedData.Add(extractedData);
            await _context.SaveChangesAsync();

            return extractedData;
        }

        private string GeneratePresignedUrl(string s3Key)
        {
            try
            {
                var request = new GetPreSignedUrlRequest
                {
                    BucketName = _configuration["AWS:hiresphere"],
                    Key = s3Key,
                    Expires = DateTime.UtcNow.AddMinutes(30) 
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
            var requestBody = new { fileUrl = fileUrl };
            var response = await _httpClient.PostAsJsonAsync("https://your-ai-api.com/analyze", requestBody);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<AIResponse>();
        }
    }

}


