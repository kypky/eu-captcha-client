using System.Text.Json;
using dsf_eu_captcha.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Http;  

namespace dsf_eu_captcha.Pages;

//[BindProperties]
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

    public CaptchaResponse? captchaResponse { get; set; }
    
    public string xjwtString = string.Empty; 

    public async Task OnGet()
    {
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "https://testapi.eucaptcha.eu/api/captchaImg?captchaLength=5")
        {
            // Headers =
            // {
            //     { HeaderNames.Accept, "application/vnd.github.v3+json" },
            //     { HeaderNames.UserAgent, "HttpRequestsSample" }
            // }
        };

        var httpClient = _httpClientFactory.CreateClient();
        var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

        if (httpResponseMessage != null)
        {
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                using var contentStream =
                    await httpResponseMessage.Content.ReadAsStreamAsync();
                
                captchaResponse = await JsonSerializer.DeserializeAsync<CaptchaResponse>(contentStream);

                //var captchaSession = HttpContext.Session.GetString("CAPTCHA");

                //Get the x-jwtString value to be used for the validation request
                if (httpResponseMessage.Headers.TryGetValues("x-jwtString", out IEnumerable<string>? values)) 
                {
                    //xjwtString = values.First();
                    if (captchaResponse != null)
                    {
                        captchaResponse.jwtString = values.First();
                        //captchaSession = new CaptchaResponse();
                        HttpContext.Session.SetString("CaptchaId", captchaResponse.captchaId);
                        HttpContext.Session.SetString("JwtString", captchaResponse.jwtString);                    
                    }                    
                }                              
            }
        }        
    }

    //public async Task<IActionResult> OnPost(string captchaAnswer, string jwt, string captchaId)
    public async Task<IActionResult> OnPost(string captchaAnswer)
    {
        var captchaId = HttpContext.Session.GetString("CaptchaId");
        var jwt = HttpContext.Session.GetString("JwtString");
        
        if (string.IsNullOrEmpty(captchaAnswer))
        {
            throw new ArgumentException($"'{nameof(captchaAnswer)}' cannot be null or empty.", nameof(captchaAnswer));
        }

        if (string.IsNullOrEmpty(jwt))
        {
            throw new ArgumentException($"'{nameof(jwt)}' cannot be null or empty.", nameof(jwt));
        }

        if (string.IsNullOrEmpty(captchaId))
        {
            throw new ArgumentException($"'{nameof(captchaId)}' cannot be null or empty.", nameof(captchaId));
        }

        var dict = new Dictionary<string, string>();
        dict.Add("captchaAnswer", captchaAnswer);
        dict.Add("useAudio", "true");
        dict.Add("x-jwtString", jwt);

        //var req = new HttpRequestMessage(HttpMethod.Post, url) { Content = new FormUrlEncodedContent(dict) };
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "https://testapi.eucaptcha.eu/api/validateCaptcha/" + captchaId)
        {
            Headers =
            {
                //{ HeaderNames.Accept, "application/vnd.github.v3+json" },
                //{ HeaderNames.UserAgent, "HttpRequestsSample" }
                //{ "x-jwtString", xjwtString}
            },

            Content = new FormUrlEncodedContent(dict)
        };

        var httpClient = _httpClientFactory.CreateClient();        
        var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

        if (httpResponseMessage.IsSuccessStatusCode)
        {
            using var contentStream =
                await httpResponseMessage.Content.ReadAsStreamAsync();
            
            var res = await JsonSerializer.DeserializeAsync<CaptchaValidateResponse>(contentStream);

            if (res != null)
            {
                if (res.responseCaptcha == "success")
                {
                    return RedirectToPage("Privacy");
                }
            }
            
        }

        return Page();
    }

    // public void OnGet()
    // {

    // }
}
