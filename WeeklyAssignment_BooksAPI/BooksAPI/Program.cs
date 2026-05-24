using BooksAPI.Configuration;
using BooksAPI.Interfaces;
using BooksAPI.Middleware;
using BooksAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 1. Register Configuration (Options Pattern)
builder.Services.Configure<PaginationOptions>(builder.Configuration.GetSection("PaginationSettings"));
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

// 2. Register Services with DI (Scoped)
builder.Services.AddScoped<IBookService, BookService>();

// 3. Add Health Checks (Including SQL Server)
builder.Services.AddHealthChecks()
    .AddSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")!);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 4. Use Custom Middlewares (Ordered correctly)
app.UseCustomMiddlewares();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// 5. Map Health Check endpoint
app.MapHealthChecks("/health");

app.Run();
