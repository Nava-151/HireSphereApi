using HireSphereApi.api;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace HireSphereApi.EndPoints
{
    public static class UserEndpoints
    {
        public static void MapUserEndPoints(WebApplication app)
        {
            var usersRoute = app.MapGroup("/users");
            usersRoute.MapGet("", async (IUserService userService) =>
            {
                var users = await userService.GetAllUsers();
                return Results.Ok(users);
            });
          
            usersRoute.MapGet("/{id}", async (int id, IUserService userService) =>
            {

                var user = await userService.GetUserById(id);
                if (user == null)
                {
                    return Results.NotFound("User not found");
                }
                return Results.Ok(user);
            });
            //regidter
            //usersRoute.MapPost("/register", async ([FromBody] UserPostModel user, IUserService userService) =>
            //{
            //    Console.WriteLine($"Received JSON: {JsonSerializer.Serialize(user)}");
            //    var createdUser = await userService.CreateUser(user);
            //    return Results.Created($"/api/users/{createdUser.Id}", createdUser);
            //});

            usersRoute.MapDelete("/{id}", async (int id, IUserService userService) =>
            {
                var isDeleted = await userService.DeleteUser(id);
                if (!isDeleted)
                {
                    return Results.NotFound("User not found");
                }
                return Results.Ok("User deleted successfully");
            });

            app.MapPut("/{id}", async (int id, [FromBody] UserPostModel user, IUserService userService) =>
            {
                var isUpdated = await userService.UpdateUser(id, user);
                if (!isUpdated)
                {
                    return Results.NotFound("User not found");
                }
                return Results.Ok("User updated successfully");
            });
        }
    }
}
