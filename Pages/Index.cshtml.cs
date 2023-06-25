using System.Text.Json;
using dsf_eu_captcha.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace dsf_eu_captcha.Pages;

public class IndexModel : PageModel
{
    //private readonly ILogger<IndexModel>? _logger;

    // public IndexModel(ILogger<IndexModel> logger)
    // {
    //     _logger = logger;
    // }

    private readonly IHttpClientFactory _httpClientFactory;

    public IndexModel(IHttpClientFactory httpClientFactory) =>
        _httpClientFactory = httpClientFactory;

    public CaptchaResponse? Captcha { get; set; }

    public async Task OnGet()
    {
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "https://testapi.eucaptcha.eu/api/captchaImg?captchaLength=6")
        {
            // Headers =
            // {
            //     { HeaderNames.Accept, "application/vnd.github.v3+json" },
            //     { HeaderNames.UserAgent, "HttpRequestsSample" }
            // }
        };

        var httpClient = _httpClientFactory.CreateClient();
        var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

        if (httpResponseMessage.IsSuccessStatusCode)
        {
            using var contentStream =
                await httpResponseMessage.Content.ReadAsStreamAsync();
            
            Captcha = await JsonSerializer.DeserializeAsync<CaptchaResponse>(contentStream);
        }
    }

    public async Task OnPost()
    {
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "https://api.eucaptcha.eu/api/validateCaptcha")
        {
            // Headers =
            // {
            //     { HeaderNames.Accept, "application/vnd.github.v3+json" },
            //     { HeaderNames.UserAgent, "HttpRequestsSample" }
            // }
        };

        var httpClient = _httpClientFactory.CreateClient();
        var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

        if (httpResponseMessage.IsSuccessStatusCode)
        {
            using var contentStream =
                await httpResponseMessage.Content.ReadAsStreamAsync();
            
            Captcha = await JsonSerializer.DeserializeAsync<CaptchaResponse>(contentStream);
        }
    }

    // public void OnGet()
    // {

    // }
}
