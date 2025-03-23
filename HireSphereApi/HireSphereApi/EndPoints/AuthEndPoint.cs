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

                var user = await userService.GetUserByEmail(loginUser.Email,loginUser.PasswordHash);
                
                if (user == null|| user.Role!=loginUser.Role)
                {
                    return Results.NotFound("User not found");
                }
                var tokenString = GenerateJwtToken(configuration);

                return Results.Ok(new { Token = tokenString ,Id=user.Id});
            });


           
            authRoute.MapPost("/register",  async ([FromBody] UserPostModel user, IUserService userService,IConfiguration configuration) =>
            {
                Console.WriteLine($"Received JSON: {JsonSerializer.Serialize(user)}");

                var u = await userService.GetUserByEmail(user.Email, user.PasswordHash);
                if(u!=null&&u.Role==user.Role)
                    return Results.BadRequest("user already exist");
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

                // יצירת המשתמש
                var createdUser = await userService.CreateUser(user);

                // יצירת ה-Token
                var token = GenerateJwtToken(configuration);

                // החזרת המשתמש + ה-Token
                return Results.Created($"/api/users/{createdUser.Id}", new
                {
                    id= createdUser.Id,
                    user = createdUser,
                    token = token
                });
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
