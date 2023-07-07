using System.Net;
using System.Text.Json;
using dsf_eu_captcha.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Net.Http.Headers;

namespace dsf_eu_captcha.Pages;

//[BindProperties]
public class IndexModel : PageModel
{
    //private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IConfiguration configuration, 
                        ILogger<IndexModel> logger) 
                        //IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _logger = logger;
        //_httpClientFactory = httpClientFactory;
    }

    public CaptchaResponse? Captcha { get; set; }
    
    public string xjwtString = string.Empty; 

    public async Task OnGet()
    {
        var requestUri = _configuration["Captcha:RequestUri"];
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri)
        {
            Headers =
            {
                { HeaderNames.Accept, "*/*" },
                { HeaderNames.UserAgent, "PostmanRuntime/7.32.3" }
            }
        };
        
        //HttpClient Handler and Proxy setup (if needed)
        HttpClientHandler httpClientHandler = new()
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        };

        try
        {
            string proxyUri = _configuration["Proxy:ProxyAddress"]!;
            var proxy = new WebProxy
            {
                Address = new Uri(proxyUri),
                BypassProxyOnLocal = false,
                UseDefaultCredentials = false
            };

            bool proxyEnabled = Boolean.Parse(_configuration["Proxy:ProxyEnabled"]!);
            if (proxyEnabled) httpClientHandler.Proxy = proxy;
        }
        catch
        {

        }

        //var httpClient = _httpClientFactory.CreateClient();
        HttpClient httpClient = new(httpClientHandler);
        
        var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

        if (httpResponseMessage != null)
        {
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                using var contentStream =
                    await httpResponseMessage.Content.ReadAsStreamAsync();
                
                Captcha = await JsonSerializer.DeserializeAsync<CaptchaResponse>(contentStream);

                //var captchaSession = HttpContext.Session.GetString("CAPTCHA");

                //Get the x-jwtString value to be used for the validation request
                if (httpResponseMessage.Headers.TryGetValues("x-jwtString", out IEnumerable<string>? values)) 
                {
                    //xjwtString = values.First();
                    if (Captcha != null)
                    {
                        Captcha.jwtString = values.First();
                        //captchaSession = new CaptchaResponse();
                        HttpContext.Session.SetString("CaptchaId", Captcha.captchaId);
                        HttpContext.Session.SetString("JwtString", Captcha.jwtString);                    
                    }                    
                }                              
            }
        }        
    }
    
    public async Task<IActionResult> OnPost(string captchaAnswer)
    {
        try 
        {
            var captchaId = HttpContext.Session.GetString("CaptchaId");
            var jwt = HttpContext.Session.GetString("JwtString");
            
            if (string.IsNullOrEmpty(captchaAnswer))
            {
                //throw new ArgumentException($"'{nameof(captchaAnswer)}' cannot be null or empty.", nameof(captchaAnswer));
                _logger.LogError($"'{nameof(captchaAnswer)}' cannot be null or empty.", nameof(captchaAnswer));
            }

            if (string.IsNullOrEmpty(jwt))
            {
                //throw new ArgumentException($"'{nameof(jwt)}' cannot be null or empty.", nameof(jwt));
                _logger.LogError($"'{nameof(jwt)}' cannot be null or empty.", nameof(jwt));
            }

            if (string.IsNullOrEmpty(captchaId))
            {
                //throw new ArgumentException($"'{nameof(captchaId)}' cannot be null or empty.", nameof(captchaId));
                _logger.LogError($"'{nameof(captchaId)}' cannot be null or empty.", nameof(captchaId));
            }

            var validationUri = _configuration["Captcha:ValidationUri"];
            var useAudio = _configuration["Captcha:UseAudio"];
            var dict = new Dictionary<string, string>
            {
                { "captchaAnswer", captchaAnswer },
                { "useAudio", useAudio! },
                { "x-jwtString", string.IsNullOrEmpty(jwt)? "" : jwt }
            };

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, validationUri + "/" + captchaId)
            {
                Headers =
                {
                    { HeaderNames.Accept, "*/*" },
                    { HeaderNames.UserAgent, "PostmanRuntime/7.32.3" } //Needed for production env
                },

                Content = new FormUrlEncodedContent(dict)
            };            
            
            //HttpClient Handler and Proxy setup (if needed)
            HttpClientHandler httpClientHandler = new()
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            try
            {
                string proxyUri = _configuration["Proxy:ProxyAddress"]!;
                var proxy = new WebProxy
                {
                    Address = new Uri(proxyUri),
                    BypassProxyOnLocal = false,
                    UseDefaultCredentials = false
                };

                bool proxyEnabled = Boolean.Parse(_configuration["Proxy:ProxyEnabled"]!);
                if (proxyEnabled) httpClientHandler.Proxy = proxy;
            }
            catch
            {

            }

            //var httpClient = _httpClientFactory.CreateClient();
            HttpClient httpClient = new(httpClientHandler);

            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();                
                var res = await JsonSerializer.DeserializeAsync<CaptchaValidateResponse>(contentStream);

                if (res != null)
                {
                    if (res.responseCaptcha == "success")
                    {
                        return RedirectToPage("Privacy");
                    }
                    else
                    {
                        _logger.LogError($"Validation Response Error: ", res.responseCaptcha);
                    }
                }                
            }
            else
            {
                _logger.LogError($"Validation Response Error: ", httpResponseMessage.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception: ", ex.ToString());
        }        

        return Page();
    }
}
