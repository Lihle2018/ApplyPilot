using JobScraper.Infrastructure.Extensions;
using JobScraper.Application.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "JobPilot API",
        Version = "v1",
        Description = "AI-powered job application assistant API for job scraping and management"
    });
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configuration
var configuration = builder.Configuration;

// Add Application Services
builder.Services.AddApplicationServices();

// Add Infrastructure Services
builder.Services.AddInfrastructureServices(
    proxyApiUrl: configuration.GetConnectionString("ProxyApiUrl") ?? "http://localhost:8000/proxy",
    searchUrl: configuration.GetConnectionString("GoogleJobsSearchUrl") ?? "https://www.google.com/search?q={0}+{1}+jobs",
    openAIApiUrl: configuration.GetConnectionString("OpenAIApiUrl") ?? "https://api.deepseek.com/v1",
    openAIToken: configuration.GetConnectionString("OpenAIToken") ?? "sk-test-token",
    mongoConnectionString: configuration.GetConnectionString("MongoDb") ?? "mongodb://localhost:27017",
    mongoDatabaseName: configuration.GetConnectionString("DatabaseName") ?? "JobPilotDb",
    jobsCollectionName: configuration.GetConnectionString("JobsCollectionName") ?? "JobListings",
    usersCollectionName: configuration.GetConnectionString("UsersCollectionName") ?? "Users"
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "JobPilot API v1");
        c.RoutePrefix = string.Empty; // Serve Swagger at root
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }))
   .WithName("HealthCheck")
   .WithTags("Health");

app.Run();
