using HireSphereApi.entities;
using Microsoft.AspNetCore.Mvc;

namespace HireSphereApi.EndPoints
{
    public static class FileEndpoints
    {
        public static void MapFileEndpoints(WebApplication app)
        {

            var fileRoute = app.MapGroup("/fiels");


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

            fileRoute.MapPost("", async ([FromBody] FilesPostModel file, IFileService fileService) =>
            {
                var uploadedFile = await fileService.UploadFile(file);
                return Results.Created($"/api/files/{uploadedFile.Id}", uploadedFile);
            });

            fileRoute.MapDelete("/{id}", async (int id, IFileService fileService) =>
            {
                var isDeleted = await fileService.DeleteFile(id);
                if (!isDeleted)
                {
                    return Results.NotFound("File not found");
                }
                return Results.Ok("File deleted successfully");
            });
        }
    }
}
