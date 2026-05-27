using MediaService.Application.Abstractions;
using MediaService.Application.Media;
using MediaService.Infrastructure.Persistence;
using MediaService.Infrastructure.Repositories;
using MediaService.Infrastructure.Storage;
using MediaService.Infrastructure.Storage.MinioStorage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// builder.Host.UseSerilog((ctx, lc) =>
//     lc.ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Auth:Authority"];
        options.Audience = builder.Configuration["Auth:Audience"];
        options.RequireHttpsMetadata = false; // فقط برای dev
    });

builder.Services.AddAuthorization();

builder.Services.AddDbContext<MediaDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<IMediaRepository, MediaRepository>()
    .AddScoped<IMediaAppService, MediaAppService>() 
    .Configure<MinioOptions>(builder.Configuration.GetSection("Storage:Minio"));

var provider = builder.Configuration["Storage:Provider"];

if (provider == "Minio")
{
    builder.Services.AddScoped<IFileStorage, MinioFileStorage>();
}
else
{
    builder.Services.AddScoped<IFileStorage, LocalFileStorage>();
}

// builder.Services.AddHealthChecks()
//     .AddDbContextCheck<MediaDbContext>();

var app = builder.Build();

// app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();  
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Media Service API");
        options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
// app.MapHealthChecks("/health");

app.Run();