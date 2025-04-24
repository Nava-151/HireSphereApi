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
            });




            extractedDataRoute.MapGet("/{id}", async (int id, IExtractedDataService extractedDataService) =>
            {
                var data = await extractedDataService.GetDataById(id);
                if (data == null)
                {
                    return Results.NotFound("Data not found");
                }
                return Results.Ok(data);
            }).RequireAuthorization();


            ////i am not sure that we need this in the site
            //extractedDataRoute.MapPost("", async ([FromBody] ExtractedDataPostModel extractedData, IExtractedDataService extractedDataService) =>
            //{
            //    var createdData = await extractedDataService.CreateData(extractedData);
            //    return Results.Created($"/api/extractedData/{createdData.Id}", createdData);
            //}).RequireAuthorization(); 

            //no need but i make it
            extractedDataRoute.MapDelete("/{id}", async (int id, IExtractedDataService extractedDataService) =>
            {
                var isDeleted = await extractedDataService.DeleteData(id);
                if (!isDeleted)
                {
                    return Results.NotFound("Data not found");
                }
                return Results.Ok("Data deleted successfully");
            }).RequireAuthorization();

            extractedDataRoute.MapPost("/filter", async ([FromBody] AiResponseDto filterParams,
    IExtractedDataService extractedDataService) =>
            {
                Console.WriteLine("in filter function ....");
                var filteredReports = await extractedDataService.GetFilteredReports(filterParams);
                return Results.Ok(filteredReports);
            }).RequireAuthorization();


        }
    }
}
