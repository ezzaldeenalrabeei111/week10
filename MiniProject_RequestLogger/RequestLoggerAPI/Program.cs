using RequestLoggerAPI.Models;
using RequestLoggerAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Options
builder.Services.Configure<RequestLoggerOptions>(builder.Configuration.GetSection("RequestLoggerSettings"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use Custom Logger Middleware
app.UseRequestLogger();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
