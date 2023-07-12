using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using dsf_eu_captcha.Models;
using Microsoft.Net.Http.Headers;

namespace dsf_eu_captcha.Services
{
    public interface ICaptchaHttpClient
    {
        Task<CaptchaResponse> GetRequest(string userAgent);
        Task<CaptchaValidateResponse> PostRequest(string captchaAnswer, string jwt, string captchaId, string userAgent);
    }
    public class CaptchaHttpClient : ICaptchaHttpClient
    {
        private readonly IConfiguration _configuration;
        
        public CaptchaHttpClient(IConfiguration configuration)
        {
            _configuration = configuration;
            //_logger = logger;
        }

        public async Task<CaptchaResponse> GetRequest(string userAgent)
        {
            CaptchaResponse? response = new();

            try
            {
                var requestUri = _configuration["Captcha:RequestUri"];
                
                HttpClientHandler httpClientHandler = new()
                {
                    //ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
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
                
                HttpClient httpClient = new(httpClientHandler);
                
                // httpClient.DefaultRequestHeaders.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue
                // {
                //     NoCache = true
                // };

                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "*/*");
                //httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0");
                //httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, "PostmanRuntime/7.32.3");
                httpClient.DefaultRequestHeaders.CacheControl = System.Net.Http.Headers.CacheControlHeaderValue.Parse("no-cache");                
                httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, userAgent);
                
                Task<HttpResponseMessage> httpResponseMessage = httpClient.GetAsync(requestUri);
                httpResponseMessage.Wait(TimeSpan.FromSeconds(10));
                
                if (httpResponseMessage != null)
                {
                    if (httpResponseMessage.IsCompleted)
                    {
                        if (httpResponseMessage.Result.StatusCode == HttpStatusCode.OK)
                        {
                            var jsonResult = await httpResponseMessage.Result.Content.ReadAsStringAsync();
                            response = JsonSerializer.Deserialize<CaptchaResponse>(jsonResult);

                            //Get the x-jwtString value to be used for the validation request
                            if (response != null)
                            {
                                if (httpResponseMessage.Result.Headers.TryGetValues("x-jwtString", out IEnumerable<string>? values)) 
                                {
                                    response.jwtString = values.First();                                                                                         
                                }
                                else
                                {
                                    Console.WriteLine($"jwtString cannot be null or empty");
                                }  
                            }
                            else
                            {
                                Console.WriteLine($"response cannot be null or empty");
                            }                            
                        }
                        else
                        {
                            Console.WriteLine($"httpResponseMessage.Result.StatusCode: " + httpResponseMessage.Result.StatusCode.ToString());
                        }                                                                                                                            
                    }
                    else
                    {
                        Console.WriteLine($"httpResponseMessage.IsCompleted=false");
                    }            
                }
                else
                {
                    Console.WriteLine($"httpResponseMessage cannot be null or empty");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
                        
            return response!;
        }

        public async Task<CaptchaValidateResponse> PostRequest(string captchaAnswer, string jwt, string captchaId, string userAgent)
        {
            CaptchaValidateResponse response = new();                        

            HttpClientHandler httpClientHandler = new()
            {
                //ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
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

            HttpClient httpClient = new(httpClientHandler);
            
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "*/*");
            //httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0");
            //httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, "PostmanRuntime/7.32.3");
            httpClient.DefaultRequestHeaders.CacheControl = System.Net.Http.Headers.CacheControlHeaderValue.Parse("no-cache");                
            httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, userAgent);


            // httpClient.DefaultRequestHeaders.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue
            // {
            //     NoCache = true
            // };

            var validationUri = _configuration["Captcha:ValidationUri"];
            //var useAudio = _configuration["Captcha:UseAudio"]!;
            var dict = new Dictionary<string, string>
            {
                { "captchaAnswer", captchaAnswer },
                { "useAudio", "true" },
                { "x-jwtString", string.IsNullOrEmpty(jwt)? "" : jwt }
            };
            
            var content = new FormUrlEncodedContent(dict);            

            //string jsonReq = DictionaryToJson(dict);
            //string jsonStringRequest = "{\"captchaAnswer\": \"" + captchaAnswer + "\",\"useAudio\": \"true\",\"x-jwtString\": \"" + jwt + "\"}";
            //StringContent postData = new(jsonStringRequest, Encoding.UTF8, "application/x-www-form-urlencoded");
            

            Task<HttpResponseMessage> httpResponseMessage = httpClient.PostAsync(validationUri + "/" + captchaId, content);
            //Task<HttpResponseMessage> httpResponseMessage = httpClient.PostAsync(request);
            httpResponseMessage.Wait(TimeSpan.FromSeconds(10));

            Console.WriteLine($"captchaId: " + captchaId);
            Console.WriteLine($"captchaAnswer: " + captchaAnswer);
            Console.WriteLine($"x-jwtString: " + jwt);
            Console.WriteLine($"user-agent: " + userAgent);
            //Console.WriteLine($"content: " + (string.IsNullOrEmpty(jsonReq)? "" : jsonReq));
                    
            if (httpResponseMessage.IsCompleted)
            {
                if (httpResponseMessage.Result.StatusCode == HttpStatusCode.OK)
                {
                    var jsonResult = await httpResponseMessage.Result.Content.ReadAsStringAsync();
                    Console.WriteLine("jsonResult: " + (string.IsNullOrEmpty(jsonResult)? "" : jsonResult));

                    response = JsonSerializer.Deserialize<CaptchaValidateResponse>(jsonResult)!;                                        
                }
                else
                {
                    Console.WriteLine($"httpResponseMessage.Result.StatusCode: " + httpResponseMessage.Result.StatusCode.ToString());
                }                                             
            }
            else
            {
                Console.WriteLine($"httpResponseMessage.IsCompleted=false");
            }

            return response;        
        }

        private static string DictionaryToJson(Dictionary<string, string> dict)
        {
            var entries = dict.Select(d =>
                string.Format("\"{0}\": \"{1}\"", d.Key, string.Join(",", d.Value)));
            return "{" + string.Join(",", entries) + "}";
        }
    }
}