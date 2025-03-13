using HireSphereApi.api.Models;
using HireSphereApi.core.entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HireSphereApi.EndPoints
{
    public static class ExtractedDataEndpoints
    {
        public static void MapExtractedDataEndPoints(WebApplication app)
        {
            var extractedDataRoute = app.MapGroup("/data");
            extractedDataRoute.MapGet("", async (IExtractedDataService extractedDataService) =>
            {
                var data = await extractedDataService.GetAllData();
                return Results.Ok(data);
            });

            extractedDataRoute.MapGet("/{id}", async (int id, IExtractedDataService extractedDataService) =>
            {
                var data = await extractedDataService.GetDataById(id);
                if (data == null)
                {
                    return Results.NotFound("Data not found");
                }
                return Results.Ok(data);
            });

            extractedDataRoute.MapPost("", async ([FromBody] ExtractedDataPostModel extractedData, IExtractedDataService extractedDataService) =>
            {
                var createdData = await extractedDataService.CreateData(extractedData);
                return Results.Created($"/api/extractedData/{createdData.Id}", createdData);
            });

            extractedDataRoute.MapDelete("/{id}", async (int id, IExtractedDataService extractedDataService) =>
            {
                var isDeleted = await extractedDataService.DeleteData(id);
                if (!isDeleted)
                {
                    return Results.NotFound("Data not found");
                }
                return Results.Ok("Data deleted successfully");
            });
            extractedDataRoute.MapGet("/filter", async ([FromBody] AIResponse filterParams, IExtractedDataService extractedDataService) =>
            {
                var filteredReports = await extractedDataService.GetFilteredReports(filterParams);
                return Results.Ok(filteredReports);
            });

            //extractedDataRoute.MapPut("/{id}", async (int id, IExtractedDataService extractedDataService) =>
            //{
            //    var isUpdated = await extractedDataService.UpdateData(id);
            //    if (!isUpdated)
            //    {
            //        return Results.NotFound("Data not found");
            //    }
            //    return Results.Ok("Data updated successfully");
            //});

        }
    }
}
