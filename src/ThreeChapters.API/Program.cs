using System.ComponentModel;
using System.Reflection;
using MediatR;
using Microsoft.OpenApi.Models;
using ThreeChapters.API.Extensions;
using UseCases.AddDailyPost;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSwaggerGen(c =>
{
    c.SupportNonNullableReferenceTypes();
    c.MapType<DateOnly>(() => new OpenApiSchema { Type = "string", Format = "date" });
    c.CustomSchemaIds(x =>
        x.GetCustomAttributes<DisplayNameAttribute>().SingleOrDefault()?.DisplayName ?? x.Name);
});
builder.Services.AddMemoryCache();
builder.Services.AddControllers();

builder.Services.AddApplicationModules(builder.Configuration);
builder.Services.AddCors(x => x.AddDefaultPolicy(policy =>
    policy.SetIsOriginAllowed(_ => true)
        .AllowCredentials()
        .AllowAnyHeader()
));
builder.Services.AddResponseCompression();
builder.Services.AddTelegramAuth();

var app = builder.Build();

app.UseResponseCompression();
app.UseFileServer();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("/index.html");

app.Run();