using HireSphereApi.core.entities;
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
            }).RequireAuthorization();

            //if i add a marks so change thr owner id to file name and date
            fileRoute.MapGet("{id}", async (int OwnerId, IFileService fileService) =>
            {
                var file = await fileService.GetFileByOwnnerId(OwnerId);
                if (file == null)
                    return Results.NotFound("File not found");

                return Results.Ok(file);
            }).RequireAuthorization();


            fileRoute.MapDelete("/{ownerId}", async (int ownerId, DataContext context, IFileService fileService) =>
            {
                bool deleted = await fileService.DeleteFile( ownerId);
                return deleted ? Results.Ok("File marked as deleted") : Results.NotFound("File not found");
            }).RequireAuthorization();


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
            }).RequireAuthorization();



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

        }
    }
}
