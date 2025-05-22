using HireSphereApi.api.Models;
using HireSphereApi.core.DTOs;
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
            }).RequireAuthorization();


            extractedDataRoute.MapGet("/{id}", async (int id, IExtractedDataService extractedDataService) =>
            {
                var data = await extractedDataService.GetDataById(id);
                if (data == null)
                {
                    return Results.NotFound("Data not found");
                }
                return Results.Ok(data);
            }).RequireAuthorization();


            extractedDataRoute.MapPost("/mark", async ([FromQuery] decimal mark, [FromQuery] int id, IExtractedDataService extractedDataService) =>
            {
                var createdData = await extractedDataService.AddMark(mark, id);
                return Results.Ok(createdData);
            }).RequireAuthorization();

           
            extractedDataRoute.MapPost("/filter", async ([FromBody] AiResponseDto filterParams,IExtractedDataService extractedDataService) =>
            {
                var filteredReports = await extractedDataService.GetFilteredReports(filterParams);
                return Results.Ok(filteredReports);
            }).RequireAuthorization();


        }
    }
}
