# DSF EU CAPTCHA Integration Demo
This is a demo application to demonstrate how to integrate (Server-Side) with the EU CAPTCHA managed API.

## What is EU CAPTCHA?
EU CAPTCHA embodies the results of ISA² Action 2018.08 EU-Captcha . A CAPTCHA is a test intended to distinguish human from machine input. The objective of this action is to offer to the Member States a CAPTCHA solution that is maintained, secure, user friendly and multilingual. It will be delivered as a component that can be operated as a service. Next to this managed service, there is also an open source project available, which you can deploy yourself.  

More information can be found at <https://code.europa.eu/eu-captcha/EU-CAPTCHA>

## Features
This web client is a demo implementation that contains the following:
* Implementation of CAPTCHA mechanism
* Alphanumeric CAPTCHA with audio transcription

## The objective of the action is to provide a CAPTCHA service that is:
* available as a component and operable as a service;
* secure;
* user friendly;
* multilingual with support for all official languages from the European Union;
* accessible by users with disabilities;
* compliant with data protection rules and best practices;
* maintained by the European Commission with continuous support for subsequent versions of the Java Virtual Machine.

## Integrations
* EU CAPTCHA Managed Service

## How it Works
* [EU CAPTCHA Managed Service User Manual v2.2.2](documentation/EU_CAPTCHA_Managed_Service_User_Manual_v2.2.2_withUrls.docx)

### Installation
```
git clone https://github.com/kypky/eu-captcha-client.git
cd eu-captcha-client
dotnet build
dotnet run
```

### Configuration (appsettings.json)
All CAPTCH related configuration settings can be set in the appsettings.json file

Test Environment:
```
"Captcha": {
    "RequestUri":"https://testapi.eucaptcha.eu/api/captchaImg?captchaLength=5",
    "ValidationUri": "https://testapi.eucaptcha.eu/api/validateCaptcha",
    "UseAudio":"true"
  }
```

Production Environment:
```
"Captcha": {
    "RequestUri":"https://api.eucaptcha.eu/api/captchaImg?captchaLength=5",
    "ValidationUri": "https://api.eucaptcha.eu/api/validateCaptcha",
    "UseAudio":"true"
  }
```

## Tech
* .NET6 C#
* Razor Pages

## NuGet Packages
* Microsoft.AspNetCore.Session

## License

MIT

**Free Software, Hell Yeah!**

#### Non-production Usage. This Software is provided for evaluation, testing, demonstration and other similar purposes. Any license and rights granted hereunder are valid only for such limited non-production purposes.
