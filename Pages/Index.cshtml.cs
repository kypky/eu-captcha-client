using System.Net;
using System.Text;
using System.Text.Json;
using dsf_eu_captcha.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Net.Http.Headers;
using dsf_eu_captcha.Services;

namespace dsf_eu_captcha.Pages;

//[BindProperties]
[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
public class IndexModel : PageModel
{
    //private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    //private readonly ILogger<IndexModel> _logger;
    private readonly ICaptchaHttpClient _captchaHttpClient;

    public IndexModel(ICaptchaHttpClient captchaHttpClient, IConfiguration configuration)
    {
        //_configuration = configuration;
        //_logger = logger;
        _captchaHttpClient = captchaHttpClient;
        _configuration = configuration;

    }

    public CaptchaResponse? Captcha { get; set; }    
    public string xjwtString = string.Empty; 

    public void OnGet()
    {
        Console.WriteLine("In IndexModel GET");
        //Remove any captcha sessions
        //HttpContext.Session.Remove("CaptchaId");
        //HttpContext.Session.Remove("JwtString");

        HttpContext.Session.SetString("CaptchaId", string.Empty);
        HttpContext.Session.SetString("JwtString", string.Empty);

        // //Get Random User Agent to test inconsistent responses from managed service
        // Random rand = new();
        // var userAgentList = _configuration.GetSection("UserAgents").Get<List<string>>();
        // int index = rand.Next(userAgentList!.Count);                
        
        var userAgent = Request.Headers["User-Agent"].ToString();
        //var userAgent = userAgentList[index].ToString();
        //HttpContext.Session.SetString("UserAgent",userAgent);

        //Captcha = await JsonSerializer.DeserializeAsync<CaptchaResponse>(contentStream);
        var res = _captchaHttpClient.GetRequest(userAgent).Result;

        //Random User Agent to test inconsistent responses


        if (res != null)
        {
            HttpContext.Session.SetString("CaptchaId", res.captchaId!);
            HttpContext.Session.SetString("JwtString", res.jwtString!);   

            //Console.WriteLine($"CaptchaId: " + res.captchaId);
            //Console.WriteLine($"JwtString: " + res.jwtString);

            Captcha = res;                                       
        }
        else
        {
            Console.WriteLine($"GetRequest response cannot be null or empty");
        }                                                                      
    }
    
    public IActionResult OnPost(string captchaAnswer)
    {
        Console.WriteLine("In IndexModel POST");

        try 
        {
            var captchaId = HttpContext.Session.GetString("CaptchaId");
            var jwt = HttpContext.Session.GetString("JwtString");
            
            //Reset the sessions
            HttpContext.Session.SetString("CaptchaId", string.Empty);
            HttpContext.Session.SetString("JwtString", string.Empty);

            var userAgent = Request.Headers["User-Agent"].ToString();
            //var userAgent = HttpContext.Session.GetString("UserAgent");
            //HttpContext.Session.SetString("UserAgent", string.Empty);


            if (string.IsNullOrEmpty(captchaAnswer))
            {
                throw new ArgumentException($"captchaAnswer cannot be null or empty.");
                //_logger.LogError($"'{nameof(captchaAnswer)}' cannot be null or empty.", nameof(captchaAnswer));
            }

            if (string.IsNullOrEmpty(jwt))
            {
                throw new ArgumentException($"jwt cannot be null or empty.");
                //_logger.LogError($"'{nameof(jwt)}' cannot be null or empty.", nameof(jwt));
            }

            if (string.IsNullOrEmpty(captchaId))
            {
                throw new ArgumentException($"captchaId cannot be null or empty.");
                //_logger.LogError($"'{nameof(captchaId)}' cannot be null or empty.", nameof(captchaId));
            }                

            var res = _captchaHttpClient.PostRequest(captchaAnswer, jwt!, captchaId!, userAgent!).Result;
                    
            if (res != null)
            {                
                if (res.responseCaptcha == "success")
                {
                    return RedirectToPage("Privacy");
                }
                else
                {
                    Console.WriteLine($"Validation Response Error: {res.responseCaptcha}", res.responseCaptcha);
                }
            }                            
            else
            {
                Console.WriteLine($"response is null");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex}", ex.ToString());
        }        

        return Page();
    }
}
