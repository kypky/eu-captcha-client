using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//KK
builder.Services.AddHttpContextAccessor();
//KK
builder.Services.AddSession();
builder.Services.AddHttpClient();
builder.Services.AddRazorPages();

// builder.Services.AddHttpClient("EUCaptcha", httpClient =>
// {       
//     //httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "*/*");
//     //httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, "HttpRequestsSample");
// });

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
