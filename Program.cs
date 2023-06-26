using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//KK
builder.Services.AddHttpContextAccessor();
//KK
builder.Services.AddSession();
builder.Services.AddHttpClient();
builder.Services.AddRazorPages();

builder.Services.AddHttpClient("EUCaptcha", httpClient =>
{
    //httpClient.BaseAddress = new Uri("https://testapi.eucaptcha.eu/api/captchaImg");

    // using Microsoft.Net.Http.Headers;
    // The GitHub API requires two headers.
    //httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/vnd.github.v3+json");
    //httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, "HttpRequestsSample");
});

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
