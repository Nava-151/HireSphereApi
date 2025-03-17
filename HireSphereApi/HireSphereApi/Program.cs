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
using HireSphereApi.core.Services;
//using Amazon.S3;
var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddAWSService<IAmazonS3>();

// Register FileService

builder.Services.AddCors();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IExtractedDataService, ExtractedDataService>();

builder.Services.AddHttpClient();
builder.Services.AddScoped<AIService,AIService>();

// ?? רישום Amazon S3 Client
builder.Services.AddSingleton<IAmazonS3, AmazonS3Client>();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

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
var connectionString = "Server=bzsuhfwgtlmuuks7ytww-mysql.services.clever-cloud.com;Port=3306;Database=bzsuhfwgtlmuuks7ytww;User=utcyh1t7uh9cxu6p;Password=0MUGD2nu8XUTwjPPiZDI;";
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthorization();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddDbContext<DataContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
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
    ServeUnknownFileTypes = true // מנסה לשרת קבצים גם אם סוגם לא מזוהה
});



app.UseCors(builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
});



// שימוש ב-Swagger במצב פיתוח בלבד
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "HireSphere API v1");

    });

}



// Add services to the container.
//builder.Services.AddOpenApi();

app.UseAuthentication();

app.UseAuthorization();

app.UseDefaultFiles(); // Enables serving default files (index.html)
app.UseStaticFiles();  // Enables serving static files (CSS, JS, etc.)




FileEndpoints.MapFileEndpoints(app);
UserEndpoints.MapUserEndPoints(app);
ExtractedDataEndpoints.MapExtractedDataEndPoints(app);
AuthEndPoint.MapAuthEndPoints(app);
app.Run();





