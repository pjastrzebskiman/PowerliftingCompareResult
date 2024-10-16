using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PowerliftingCompareResult.Models;

var builder = WebApplication.CreateBuilder(args);
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ResultContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ResultContextConnectionString")));

builder.Services.Configure<CsvSettings>(builder.Configuration.GetSection("ImportFormCsv"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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
