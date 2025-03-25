using Amazon.Runtime.Internal;
using Amazon.S3;
using Amazon.S3.Model;
using HireSphereApi.core.entities;
using HireSphereApi.core.services;
using HireSphereApi.Data;
using HireSphereApi.entities;
using HireSphereApi.Service.Iservice;
using iText.Commons.Utils;
using Microsoft.AspNetCore.Mvc;

namespace HireSphereApi.EndPoints
{
    public static class FileEndpoints
    {
        public static void MapFileEndpoints(WebApplication app)
        {

            var fileRoute = app.MapGroup("/files");


            fileRoute.MapGet("", async (IFileService fileService) =>
            {
                var files = await fileService.GetAllFiles();
                return Results.Ok(files);
            });

            fileRoute.MapGet("{id}", async (int id, IFileService fileService) =>
            {
                var file = await fileService.GetFileById(id);
                if (file == null)
                {
                    return Results.NotFound("File not found");
                }
                return Results.Ok(file);
            });



            fileRoute.MapDelete("/{fileId}", async (int fileId, int ownerId, DataContext context, IFileService fileService) =>
            {
                bool deleted = await fileService.DeleteFile(fileId, ownerId);
                return deleted ? Results.Ok("File marked as deleted") : Results.NotFound("File not found");
            });


            fileRoute.MapGet("/download", async ([FromQuery] string fileName, IS3Service fileService) =>
            {
                var url = await fileService.GeneratePresignedUrlToDownload(fileName);
                using var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Problem("Failed to fetch the file.");
                }

                var fileBytes = await response.Content.ReadAsByteArrayAsync();
                var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";

                return Results.File(fileBytes, contentType, fileName);
            });

            //*****remove the // to angular no need of it
            //fileRoute.MapGet("/download/{fileName}", async (string fileName, IS3Service yourService) =>
            //{
            //    var fileUrl = await yourService.GeneratePresignedUrlToDownload(fileName);

            //    using var httpClient = new HttpClient();
            //    var response = await httpClient.GetAsync(fileUrl);

            //    if (!response.IsSuccessStatusCode)
            //    {
            //        return Results.Problem("Failed to fetch the file.");
            //    }

            //    // יצירת Stream מהתגובה
            //    var fileStream = await response.Content.ReadAsStreamAsync();
            //    return fileStream;

            //});


            fileRoute.MapGet("/upload", async ([FromQuery] string fileName, IS3Service fileService) =>
            {
                var url = await fileService.GeneratePresignedUrlToUpload(fileName);
                return url;

            }).RequireAuthorization();


            //fileRoute.MapPost("/resume/analyze", async ([FromBody] ResumeAnalyzeRequest request, IS3Service s3Service, AIService aiService, TextExtractionService textService, DataContext db) =>
            //{
            //    if (string.IsNullOrEmpty(request.S3Key))
            //        return Results.BadRequest("Invalid S3 key.");
            //    //להעביר לפונקציה אחת משותפת
            //    //הורדת הקובץ מ - S3
            //    var fileUrl = await s3Service.GeneratePresignedUrlToDownload(request.S3Key);
            //    if (fileUrl == null)
            //        return Results.BadRequest("File not found in S3.");
            //    using var httpClient = new HttpClient();
            //    var response = await httpClient.GetAsync(fileUrl);

            //    if (!response.IsSuccessStatusCode)
            //    {
            //        return Results.Problem("Failed to fetch the file.");
            //    }

            //    // יצירת Stream מהתגובה
            //    var fileStream = await response.Content.ReadAsStreamAsync();
            //    Console.WriteLine(fileStream.ToString());
            //    // חילוץ טקסט מהקובץ
            //    string extractedText = textService.ExtractTextFromFile(fileStream, request.S3Key);
            //    Console.WriteLine(extractedText);
            //    if (string.IsNullOrWhiteSpace(extractedText))
            //    return Results.BadRequest("Failed to extract text from file.");


            //    // ניתוח באמצעות GPT
            //    var aiResponse = await aiService.AnalyzeResumeAsync(extractedText);

            //    if (aiResponse == null)
            //        return Results.StatusCode(500);

            //    // שמירת תוצאות במסד הנתונים
            //    db.AIResponses.Add(aiResponse);
            //    await db.SaveChangesAsync();

            //    var extractedData = new ExtractedDataEntity
            //    {
            //        CandidateId = request.UserId,
            //        FileKey = request.S3Key,
            //        CreatedAt = DateTime.UtcNow,
            //        UpdatedAt = DateTime.UtcNow,
            //        IdResponse = aiResponse.Id
            //    };

            //    db.ExtractedData.Add(extractedData);
            //    await db.SaveChangesAsync();

            //    return Results.Ok(new { extractedData.Id });
            //}).RequireAuthorization();
            fileRoute.MapPost("/resume/analyze", async ([FromBody] ResumeAnalyzeRequest request, IS3Service s3Service, AIService aiService, TextExtractionService textService, DataContext db) =>
            {
                try
                {
                    if (string.IsNullOrEmpty(request.S3Key))
                        return Results.BadRequest("Invalid S3 key.");

                    // הורדת הקובץ מ - S3
                    var fileUrl = await s3Service.GeneratePresignedUrlToDownload(request.S3Key);
                    Console.WriteLine($"Generated S3 URL: {fileUrl}");
                    if (fileUrl == null)
                        return Results.BadRequest("File not found in S3.");

                    using var httpClient = new HttpClient();
                    var response = await httpClient.GetAsync(fileUrl);
                    Console.WriteLine($"HTTP Response Status: {response.StatusCode}");

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Error fetching file: {errorMessage}");
                        return Results.Problem($"Failed to fetch the file. Status Code: {response.StatusCode}, Error: {errorMessage}");
                    }


                    // יצירת Stream מהתגובה
                    using var fileStream = await response.Content.ReadAsStreamAsync();

                    // העתקת הזרם ל-MemoryStream כדי למנוע בעיות קריאה חוזרת
                    using var memoryStream = new MemoryStream();
                    await fileStream.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    Console.WriteLine($"File stream length: {memoryStream.Length}");

                    // חילוץ טקסט מהקובץ
                    string extractedText = textService.ExtractTextFromFile(memoryStream, request.S3Key);
                    Console.WriteLine($"Extracted text: {extractedText}");
                    if (string.IsNullOrWhiteSpace(extractedText))
                        return Results.BadRequest("Failed to extract text from file.");

                    // ניתוח באמצעות GPT
                    var aiResponse = await aiService.AnalyzeResumeAsync(extractedText);
                    if (aiResponse == null)
                        return Results.StatusCode(500);

                    Console.WriteLine("before saving  "+aiResponse.Languages);
                    // שמירת תוצאות במסד הנתונים
                    try
                    {
                        db.AIResponses.Add(aiResponse);
                        await db.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error saving to database: " + ex.Message);
                    }


                    var extractedData = new ExtractedDataEntity
                    {
                        CandidateId = request.UserId,
                        FileKey = request.S3Key,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IdResponse = aiResponse.Id
                    };

                    db.ExtractedData.Add(extractedData);
                    await db.SaveChangesAsync();

                    return Results.Ok(new { extractedData.Id });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    return Results.Problem($"Unexpected error: {ex.Message}");
                }
            }).RequireAuthorization();




        }
    }
}
