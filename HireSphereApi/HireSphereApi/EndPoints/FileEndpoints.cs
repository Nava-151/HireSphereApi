using HireSphereApi.core.entities;
using HireSphereApi.Data;
using HireSphereApi.entities;
using HireSphereApi.Service.Iservice;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            }).RequireAuthorization();


            fileRoute.MapGet("{id}", async (int OwnerId, IFileService fileService) =>
            {
                var file = await fileService.GetFileByOwnnerId(OwnerId);
                if (file == null)
                    return Results.NotFound("File not found");

                return Results.Ok(file);
            }).RequireAuthorization();


            fileRoute.MapDelete("/{ownerId}", async (int ownerId, DataContext context, IFileService fileService) =>
            {
                try
                {
                    bool deleted = await fileService.DeleteFile(ownerId);
                    return deleted ? Results.Ok("File deleted successfully.") : Results.Problem("File deletion failed.");
                }
                catch (FileNotFoundException ex)
                {
                    return Results.NotFound(ex.Message);
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(ex.Message);
                }

            });


            fileRoute.MapGet("/view", async ([FromQuery] int ownerId, IS3Service s3Service, IFileService fileService) =>
            {
                var file = await fileService.GetFileByOwnnerId(ownerId);
                if (file == null)
                    return Results.BadRequest("no file uploaded");
                var url = await s3Service.GeneratePresignedUrlToDownload(file.FileName);

                return Results.Ok(url);

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
                string contentType = response.Content.Headers.ContentType?.ToString();
                if (string.IsNullOrEmpty(contentType) || contentType == "application/octet-stream")
                {
                    var extension = Path.GetExtension(fileName).ToLowerInvariant();
                    contentType = extension switch
                    {
                        ".pdf" => "application/pdf",
                        ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                        _ => "application/octet-stream"
                    };
                }


                return Results.File(fileBytes, contentType, fileName);
            });
            //.RequireAuthorization();



            fileRoute.MapGet("/upload", async ([FromQuery] string fileName, IS3Service fileService) =>
            {
                var url = await fileService.GeneratePresignedUrlToUpload(fileName);
                return url;

            }).RequireAuthorization();

            fileRoute.MapPost("", async ([FromBody] FilesPostModel request, IFileService fileService) =>
            {
                return await fileService.AddFile(request);
            }).RequireAuthorization();


            fileRoute.MapPost("/resume/analyze", async ([FromBody] ResumeAnalyzeRequest request, IFileService fileService) =>
            {
                return await fileService.AnalyzeResumeAsync(request);
            }).RequireAuthorization();

            //fileRoute.MapGet("/download/{fileId}", async (int fileId, DataContext context) =>
            //{
            //    var fileEntity = await context.Files.FindAsync(fileId);
            //    if (fileEntity == null)
            //    {
            //        return Results.NotFound();
            //    }

            //    // הניתוב ב-S3
            //    string filePath = fileEntity.S3Key;

            //    // יצירת URL להורדה ישירה
            //    var fileUrl = GenerateS3DownloadUrl(filePath);

            //    return Results.Ok(new { downloadUrl = fileUrl });
            //});

            //// פונקציה ליצירת URL להורדה ישירה
            //string GenerateS3DownloadUrl(string filePath)
            //{
            //    string yourBucketName = "hiresphere"; 
            //    return $"https://s3.amazonaws.com/{yourBucketName}/{filePath}";
            //}

        }
    }
}
