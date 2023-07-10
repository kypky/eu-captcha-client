using System.Net;
using System.Text;
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
        //Remove any captcha sessions
        HttpContext.Session.Remove("CaptchaId");
        HttpContext.Session.Remove("JwtString");

        var requestUri = _configuration["Captcha:RequestUri"];
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri)
        {
            Headers =
            {
                { HeaderNames.Accept, "*/*" },
                { HeaderNames.UserAgent, HttpContext.Request.Headers["User-Agent"].ToString() }
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
                using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                
                Captcha = await JsonSerializer.DeserializeAsync<CaptchaResponse>(contentStream);

                if (Captcha != null)
                {
                    //Get the x-jwtString value to be used for the validation request
                    if (httpResponseMessage.Headers.TryGetValues("x-jwtString", out IEnumerable<string>? values)) 
                    {
                        Captcha.jwtString = values.First();
                        //captchaSession = new CaptchaResponse();
                        HttpContext.Session.SetString("CaptchaId", Captcha.captchaId);
                        HttpContext.Session.SetString("JwtString", Captcha.jwtString);  

                        Console.WriteLine($"Get: Captcha.captchaId: {Captcha.captchaId}", Captcha.captchaId);
                        Console.WriteLine($"Get: Captcha.jwtString: {Captcha.jwtString}", Captcha.jwtString);                                                
                    }
                    else
                    {
                        Console.WriteLine($"jwtString cannot be null or empty");
                    }
                }
                else
                {
                    Console.WriteLine($"Captcha cannot be null or empty");
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
                { "useAudio", "true" },
                { "x-jwtString", string.IsNullOrEmpty(jwt)? "" : jwt }
            };

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, validationUri + "/" + captchaId)
            {
                Headers =
                {
                    { HeaderNames.Accept, "*/*" },
                    { HeaderNames.UserAgent, HttpContext.Request.Headers["User-Agent"].ToString() }
                    //{ HeaderNames.UserAgent, "PostmanRuntime/7.32.3" } //Needed for production env
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

                Console.WriteLine($"proxyEnabled: {0}", proxyEnabled.ToString());
            }
            catch
            {

            }
            
            //Log Information
            Console.WriteLine($"Post: captchaAnswer: {captchaAnswer}", captchaAnswer);
            Console.WriteLine($"Post: x-jwtString: {jwt}", jwt);       

            //var httpClient = _httpClientFactory.CreateClient();
            HttpClient httpClient = new(httpClientHandler);

            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
                    
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();    

                StreamReader readStream = new(contentStream, Encoding.UTF8);
                string contentString = readStream.ReadToEnd();
                //Console.WriteLine($"contentString: {contentString}", contentString);

                var res = JsonSerializer.Deserialize<CaptchaValidateResponse>(contentString)!.responseCaptcha;

                try
                {
                    string jsonString = JsonSerializer.Serialize(res);
                    Console.WriteLine($"res: {jsonString}", jsonString);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"res: {ex} exception", ex.ToString());
                }

                if (res != null)
                {
                    if (res == "success")
                    {
                        return RedirectToPage("Privacy");
                    }
                    else
                    {
                        Console.WriteLine($"Validation Response Error: {res}", res);
                    }
                }                
            }
            else
            {
                Console.WriteLine($"Validation Response Error: {httpResponseMessage.StatusCode}", httpResponseMessage.StatusCode);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex}", ex.ToString());
        }        

        return Page();
    }
}
