using HireSphereApi.api;
using HireSphereApi.Service.Iservice;
using Microsoft.AspNetCore.Mvc;

namespace HireSphereApi.EndPoints
{
    public class AiResponseEndPoint
    {
        public static void MapAiEndPoints(WebApplication app)
        {
            var aiRoute = app.MapGroup("/ai");


            aiRoute.MapGet("/{id}", async (int id, IAIService aIService) =>
            {
                var data = await aIService.GetAIResponse(id);
                if (data == null)
                {
                    return Results.NotFound("response not found");
                }
                return Results.Ok(data);
            });

            aiRoute.MapPost("/chat", async ([FromBody] ChatMessageDto message, IAIService aIService) =>
            {
                if (string.IsNullOrWhiteSpace(message.Message))
                    return Results.BadRequest("Message is required.");

                var result = await aIService.ChatWithAiAsync(message.Message);
                return Results.Ok(result);
            });
        }
    }

    public class ChatMessageDto
    {
        public string Message { get; set; } = string.Empty;
    }
}
