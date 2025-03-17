using Amazon.S3;
using Amazon.S3.Model;
using HireSphereApi.core.entities;
using HireSphereApi.Data;
using HireSphereApi.entities;
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



            app.MapDelete("/{fileId}", async (int fileId,int ownerId, DataContext context, IFileService fileService) =>
            {
                bool deleted = await fileService.DeleteFile(fileId,ownerId);
                return deleted ? Results.Ok("File marked as deleted") : Results.NotFound("File not found");
            });


            //for dowloading the file cjeck how i will have the fileKey
            app.MapGet("/get-url", (string fileKey, IFileService fileService) =>
            {
                var url = fileService.GetPresignedUrl(fileKey);
                return Results.Ok(new { url });
            });


            app.MapPost("/upload", async ([FromBody]FileEntity file, IFileService fileService) =>
            {
                var savedFile = await fileService.UploadFile(file);
                return Results.Ok(savedFile);
            });



        }
    }
}
