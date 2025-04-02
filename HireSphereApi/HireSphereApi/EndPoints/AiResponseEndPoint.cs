using HireSphereApi.api;
using HireSphereApi.Service.Iservice;
using Microsoft.AspNetCore.Mvc;

namespace HireSphereApi.EndPoints
{
    public class AiResponseEndPoint
    {
        public static void MapAiEndPoints(WebApplication app)
        {
            var aiRoute = app.MapGroup("/aiResponse");


            aiRoute.MapGet("/{id}", async (int id, IAIService aIService) =>
            {
                var data = await aIService.GetAIResponse(id);
                if (data == null)
                {
                    return Results.NotFound("response not found");
                }
                return Results.Ok(data);
            });
        }
    }
    }
