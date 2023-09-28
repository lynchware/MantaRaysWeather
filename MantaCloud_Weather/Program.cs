
using MantaRays_Weather.Interfaces;
using MantaRays_Weather.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IForecastAPIService, ForecastAPIService>();

var apiConfigs = builder.Configuration.GetSection("APIs").GetChildren();

foreach (var apiConfig in apiConfigs)
{
    var apiName = apiConfig.Key;
    var uri = apiConfig.GetValue<string>("Uri");
    var userAgent = apiConfig.GetValue<string>("UserAgent");
    var token = apiConfig.GetValue<string>("Token"); // Will be null if not provided

    builder.Services.AddHttpClient(apiName, client =>
    {
        client.BaseAddress = new Uri(uri);
        if (!string.IsNullOrEmpty(userAgent))
        {
            client.DefaultRequestHeaders.Add("User-Agent", userAgent);
        }

        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }
    });
}
builder.Services.AddApplicationInsightsTelemetry(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
