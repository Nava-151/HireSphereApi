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
using Microsoft.AspNetCore.HttpOverrides;



var builder = WebApplication.CreateBuilder(args);

Env.Load("keys.env");

string accessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID") ?? throw new InvalidOperationException("AWS_ACCESS_KEY_ID is missing."); ;
string secretKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY") ?? throw new InvalidOperationException("AWS_SECRET_ACCESS_KEY is missing.");
string region = Environment.GetEnvironmentVariable("AWS_REGION") ?? throw new InvalidOperationException("AWS_REGION is missing.");

builder.Services.AddCors();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", builder =>
    {
        builder.WithOrigins("http://localhost:5173", "http://localhost:4200","https://hiresphereangular.onrender.com","https://hirespherereact.onrender.com","https://hiresphereapi.onrender.com")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});
builder.Services.AddSignalR();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IS3Service, S3Service>();

builder.Services.AddScoped<IExtractedDataService, ExtractedDataService>();

builder.Services.AddHttpClient();
builder.Services.AddScoped<IAIService, AIService>();

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

builder.Services.AddScoped<AIService>(); 

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
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
    mySqlOptions => mySqlOptions.EnableRetryOnFailure());

});
if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("Connection string is not found or empty.");
}
else
{
    Console.WriteLine($"Connection string loaded: {connectionString}");
}


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
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

var app = builder.Build();
app.UseStaticFiles(new StaticFileOptions
{
    ServeUnknownFileTypes = true
});

app.UseCors("AllowFrontend");
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "HireSphere API v1");
    c.RoutePrefix = string.Empty;

});
app.UseAuthentication();
app.UseAuthorization();
app.UseDefaultFiles(); 
app.UseStaticFiles();  
app.MapHub<VideoCallHub>("/videoCallHub");
app.MapGet("/", () => "Hello World!");
FileEndpoints.MapFileEndpoints(app);
AiResponseEndPoint.MapAiEndPoints(app);
UserEndpoints.MapUserEndPoints(app);
ExtractedDataEndpoints.MapExtractedDataEndPoints(app);
AuthEndPoint.MapAuthEndPoints(app);

app.Run();





