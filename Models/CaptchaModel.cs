namespace dsf_eu_captcha.Models;

public class CaptchaResponse
    {
        public required string captchaId { get; set; }
        public required string captchaImg { get; set; }
        public required string captchaType { get; set; }
        public required object captchaQuestion { get; set; }
        public int max { get; set; }
        public int min { get; set; }
        public string? audioCaptcha { get; set; }
        public string? jwtString { get; set; }
    }

   public class CaptchaValidateResponse
    {
        public string? responseCaptcha { get; set; }
    }