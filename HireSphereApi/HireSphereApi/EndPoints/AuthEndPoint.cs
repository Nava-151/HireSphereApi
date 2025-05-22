//using HireSphereApi.api;


using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using DocumentFormat.OpenXml.Spreadsheet;
using HireSphereApi.api;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using HireSphereApi.core.DTO;


namespace HireSphereApi.EndPoints
{
    public static class AuthEndPoint
    {
        public static void MapAuthEndPoints(WebApplication app)
        {
            var authRoute = app.MapGroup("/auth");

            authRoute.MapPost("/login", async ([FromBody] LoginUser loginUser, IUserService userService, IConfiguration configuration) =>
            {

                var user = await userService.GetUserByEmail(loginUser.Email, loginUser.PasswordHash);

                if (user == null)
                //|| user.Role!=loginUser.Role)
                {
                    return Results.NotFound("User not found");
                }

                var tokenString = GenerateJwtToken(configuration, user);

                return Results.Ok(new { token = tokenString, id = user.Id });
            });



            authRoute.MapPost("/register", async ([FromBody] UserPostModel user, IUserService userService, IConfiguration configuration) =>
            {

                var u = await userService.GetUserByEmail(user.Email, user.PasswordHash);
                if (u != null && u.Role == user.Role)
                    return Results.BadRequest("user already exist");
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

                var createdUser = await userService.CreateUser(user);

                var token = GenerateJwtToken(configuration, createdUser);

                return Results.Created($"/api/users/{createdUser.Id}", new
                {
                    id = createdUser.Id,
                    user = createdUser,
                    token = token
                });
            });
        }

        private static string GenerateJwtToken(IConfiguration configuration, UserDto user)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var expirationTime = DateTime.UtcNow.AddDays(7);

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("FullName", user.FullName ?? "")
            };
          
           var tokenOptions = new JwtSecurityToken(
               issuer: configuration["Jwt:Issuer"],             
               audience: configuration["Jwt:Audience"],         
               claims: claims,
               expires: expirationTime,
               signingCredentials: signinCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

    }
}
