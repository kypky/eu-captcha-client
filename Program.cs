using System.Net;
using dsf_eu_captcha.Services;
using Microsoft.Net.Http.Headers;


var builder = WebApplication.CreateBuilder(args);

//var configuration = builder.Configuration;

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
// Add services to the container.
//KK
builder.Services.AddHttpContextAccessor();
//KK
builder.Services.AddSession();
builder.Services.AddHttpClient();
builder.Services.AddRazorPages();

//Captcha HttpClient
builder.Services.AddSingleton<ICaptchaHttpClient, CaptchaHttpClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//KK
app.UseSession();  

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
