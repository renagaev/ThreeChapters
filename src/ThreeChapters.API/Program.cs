using ThreeChapters.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddApplicationModules(builder.Configuration);
builder.Services.AddCors(x => x.AddDefaultPolicy(policy =>
    policy.SetIsOriginAllowed(_ => true)
        .AllowCredentials()
        .AllowAnyHeader()
));
builder.Services.AddResponseCompression();

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
app.MapControllers();

app.Run();