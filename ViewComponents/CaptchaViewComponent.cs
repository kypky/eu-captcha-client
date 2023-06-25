using dsf_eu_captcha.Models;
using Microsoft.AspNetCore.Mvc;

namespace HttpRequestsSample.ViewComponents;

public class CaptchaViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(CaptchaResponse? captcha) =>
        View(captcha);
}