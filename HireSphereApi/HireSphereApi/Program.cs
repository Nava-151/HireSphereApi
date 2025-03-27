using HireSphereApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;
using HireSphereApi.core;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using HireSphereApi.EndPoints;
using Amazon.S3;
using HireSphereApi.core.services;
using DotNetEnv;
using HireSphereApi.Service.Iservice;
using OpenAI;


var builder = WebApplication.CreateBuilder(args);

Env.Load("keys.env");

// מקבל את הערכים
string accessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID") ?? throw new InvalidOperationException("AWS_ACCESS_KEY_ID is missing."); ;
string secretKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");
string region = Environment.GetEnvironmentVariable("AWS_REGION");

Console.WriteLine("accessKey "+accessKey+" secretKey "+secretKey+" region "+region);
builder.Services.AddCors();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IS3Service,S3Service>();

builder.Services.AddScoped<IExtractedDataService, ExtractedDataService>();

builder.Services.AddHttpClient();
builder.Services.AddScoped<IAIService,AIService>();

builder.Services.AddSingleton<IAmazonS3, AmazonS3Client>();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

builder.Services.AddSingleton<OpenAIClient>(sp =>
{
    var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
    if (string.IsNullOrEmpty(apiKey))
    {
        throw new InvalidOperationException("OPENAI_API_KEY environment variable is not set.");
    }

    return new OpenAIClient(apiKey);
});

builder.Services.AddScoped<AIService>(); // Register your AIService



builder.Services.AddScoped<TextExtractionService>();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer Authentication with JWT Token",
        Type = SecuritySchemeType.Http
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthorization();
builder.Services.AddAutoMapper(typeof(MappingProfile));


builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});


builder.Services.Configure<JsonOptions>(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
});


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidAudience = builder.Configuration["JWT:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
        };
    });


var app = builder.Build();
app.UseStaticFiles(new StaticFileOptions
{
    ServeUnknownFileTypes = true 
});


app.UseCors(builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
});

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "HireSphere API v1");
    c.RoutePrefix = string.Empty; 

});

app.UseAuthentication();
app.UseAuthorization();

app.UseDefaultFiles(); // Enables serving default files (index.html)
app.UseStaticFiles();  // Enables serving static files (CSS, JS, etc.)


app.MapGet("/", () => "Hello World!");

FileEndpoints.MapFileEndpoints(app);
UserEndpoints.MapUserEndPoints(app);
ExtractedDataEndpoints.MapExtractedDataEndPoints(app);
AuthEndPoint.MapAuthEndPoints(app);

app.Run();





