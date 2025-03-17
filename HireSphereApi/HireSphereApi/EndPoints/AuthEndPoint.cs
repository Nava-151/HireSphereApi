//using HireSphereApi.api;


using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;
using HireSphereApi.api;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace HireSphereApi.EndPoints
{
    public static class AuthEndPoint
    {
        public static void MapAuthEndPoints(WebApplication app)
        {
            var authRoute = app.MapGroup("/auth");

            authRoute.MapPost("/login", async ([FromBody]LoginUser loginUser, IUserService userService, IConfiguration configuration) =>
            {

                var user = await userService.GetUserByEmail(loginUser);
                if (user == null)
                {
                    return Results.NotFound("User not found");
                }
                var tokenString = GenerateJwtToken(configuration);

                return Results.Ok(new { Token = tokenString ,Id=user.Id});
            });
            authRoute.MapPost("/register", async ([FromBody] UserPostModel user, IUserService userService) =>
            {
                Console.WriteLine($"Received JSON: {JsonSerializer.Serialize(user)}");
               user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
                var createdUser = await userService.CreateUser(user);
                return Results.Created($"/api/users/{createdUser.Id}", createdUser);
            });
        }

        private static string GenerateJwtToken(IConfiguration configuration)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var tokenOptions = new JwtSecurityToken(
                issuer: configuration["JWT:Issuer"],
                audience: configuration["JWT:Audience"],
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: signinCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }
    }
}
