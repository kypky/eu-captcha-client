namespace dsf_eu_captcha.Models;

public class CaptchaResponse
    {
        public string? captchaId { get; set; }
        public string? captchaImg { get; set; }
        public string? captchaType { get; set; }
        public object? captchaQuestion { get; set; }
        public int Max { get; set; }
        public int Min { get; set; }
        public string? audioCaptcha { get; set; }
        public string? jwtString { get; set; }
    }

   public class CaptchaValidateResponse
    {
        public string? responseCaptcha { get; set; }
    }