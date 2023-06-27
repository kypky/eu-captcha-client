# DSF EU CAPTCHA Integration Demo
This is a .NET 6 Razor Pages demo application to demonstrate how to integrate (Server-Side) with the EU CAPTCHA managed API.

## What is EU CAPTCHA?
EU CAPTCHA embodies the results of ISA² Action 2018.08 EU-Captcha . A CAPTCHA is a test intended to distinguish human from machine input. The objective of this action is to offer to the Member States a CAPTCHA solution that is maintained, secure, user friendly and multilingual. It will be delivered as a component that can be operated as a service. Next to this managed service, there is also an open source project available, which you can deploy yourself.  

A CAPTCHA is an essential component of information systems dealing specifically with human users, such as citizens. Having an open source secure and user-friendly CAPTCHA prevents from having to acquire or implement a specific one in several information systems. It allows Member States to offer a consistent user experience throughout public services in Europe for a step-in administrative process that are perceived as complicated by many users. The EU CAPTCHA solution solves a common problem in a consistent and cost-effective way and solves a common problem that is not related to any particular sector or EU Member State.

More information can be found at <https://code.europa.eu/eu-captcha/EU-CAPTCHA>

## Features
This web client is a demo implementation that contains the following:
* Implementation of CAPTCHA mechanism
* Alphanumeric CAPTCHA with audio transcription

## Objective:  Integrate with a CAPTCHA service that is:
* available as a component and operable as a service;
* secure;
* user friendly;
* multilingual with support for all official languages from the European Union;
* accessible by users with disabilities;
* compliant with data protection rules and best practices;
* maintained by the European Commission with continuous support for subsequent versions.

## Integrations
* EU CAPTCHA Managed Service

To integrate with an instance of EU CAPTCHA Managed Service these environments are available:  
```
QA environment: https://testapi.eucaptcha.eu/
PRD environment: https://api.eucaptcha.eu/ 
```

## How it Works
CAPTCHA Managed Service Implementation Manual is for the reference of developers wanting to use the EU CAPTCHA Managed Service.  
* [EU CAPTCHA Managed Service User Manual v2.2.2](documentation/EU_CAPTCHA_Managed_Service_User_Manual_v2.2.2_withUrls.docx)

### Installation
```
git clone https://github.com/kypky/eu-captcha-client.git
cd eu-captcha-client
dotnet build
dotnet run
```

### Configuration (appsettings.json)
All CAPTCHA related configuration settings can be set in the appsettings.json file

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

