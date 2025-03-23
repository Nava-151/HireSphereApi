using Amazon.Runtime.Internal;
using Amazon.S3;
using Amazon.S3.Model;
using HireSphereApi.core.entities;
using HireSphereApi.core.services;
using HireSphereApi.Data;
using HireSphereApi.entities;
using HireSphereApi.Service.Iservice;
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
                return Results.Ok(new { url });

            }).RequireAuthorization();


            fileRoute.MapGet("/upload", async ([FromQuery] string fileName, IS3Service fileService) =>
            {
                var url = await fileService.GeneratePresignedUrlToUpload(fileName);
                return url;

            }).RequireAuthorization();


            fileRoute.MapPost("/resume/analyze", async ([FromBody] ResumeAnalyzeRequest request, IS3Service s3Service, AIService aiService, TextExtractionService textService, DataContext db) =>
            {
                if (string.IsNullOrEmpty(request.S3Key))
                    return Results.BadRequest("Invalid S3 key.");

                // הורדת הקובץ מ-S3
                var fileStream = await s3Service.DownloadFileAsync(request.S3Key);
                if (fileStream == null)
                    return Results.BadRequest("File not found in S3.");

                // חילוץ טקסט מהקובץ
                string extractedText = textService.ExtractTextFromFile(fileStream, request.S3Key);
                Console.WriteLine(extractedText);
                if (string.IsNullOrWhiteSpace(extractedText))
                    return Results.BadRequest("Failed to extract text from file.");

                // ניתוח באמצעות GPT
                var aiResponse = await aiService.AnalyzeResumeAsync(extractedText);
                if (aiResponse == null)
                    return Results.StatusCode(500);

                // שמירת תוצאות במסד הנתונים
                db.AIResponses.Add(aiResponse);
                await db.SaveChangesAsync();

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
            }).RequireAuthorization();



        }
    }
}
