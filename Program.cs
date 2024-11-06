using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PowerliftingCompareResult.Models;

var builder = WebApplication.CreateBuilder(args);
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
Console.WriteLine($"Port from environment variable: {port}");
// Add services to the container.

string dbHost = Environment.GetEnvironmentVariable("YOUR_HOST");
string dbPort = Environment.GetEnvironmentVariable("YOUR_PORT"); 
string dbName = Environment.GetEnvironmentVariable("YOUR_DATABASE");
string dbUser = Environment.GetEnvironmentVariable("YOUR_USERNAME");
string dbPassword = Environment.GetEnvironmentVariable("YOUR_PASSWORD");

string connectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword}";
Console.WriteLine($"Connection strong to db PJ : : {connectionString}");

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ResultContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.Configure<CsvSettings>(builder.Configuration.GetSection("ImportFormCsv"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");


var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseHsts();

}

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapControllers();



app.MapFallbackToFile("index.html");

app.Run();
