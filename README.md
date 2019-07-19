---
languages:
- javascript
- csharp
page_type: sample
products:
- azure
- azure-cognitive-services
description: "The Intelligent Mission app is intended to demonstrate how organizations can use Cognitive Services in a Government-focused scenario."
---

# Intelligent Mission

The Intelligent Mission app is intended to demonstrate how organizations
can use Cognitive Services in a Government-focused scenario. The Intelligent
Mission app can be seen in [this Channel 9 video](https://channel9.msdn.com/blogs/Azure-Government/Cognitive-Services-on-Azure-Government-Intelligent-Mission) (though not a complete demo).

The target environment for this solution is [Azure Government](https://azure.microsoft.com/en-us/overview/clouds/government/).

The solution includes:

* Face Detection
* Face Identification (i.e., Recognition)
* Object recognition in images
* OCR in images
* Sentiment Analysis
* Text Translation
* Speaker Identification

Other technologies include:

* Azure Active Directory
* Azure Blob Storage
* Azure Cosmos DB
* ASP.NET Core
* Angular 4

# Building and Running the Solution

The solution is divided into 2 major areas:
* IntelligentMission.Web - This is an ASP.NET Core project that hosts the website, provides the Web API for the UI, and Authentications with Azure Active Directory.
* intelligent-mission-mg - This is an Angular app (originally generated with the Angular CLI, so make sure you already have the Angular CLI installed).

```
npm install -g @angular/cli
```

To build and run the solution locally:

First, build the Angular project:

```
cd intelligent-mission-ng
npm install
ng build --prod
```

The `.angular-cli.json` file contains: `"outDir": "../IntelligentMission.Web/wwwroot"` - this builds the Angular project and puts it in the 
`wwwroot` folder of the ASP.NET Core project.

At this point, you can Build the ASP.NET Core project (which will of course restore the NuGet packages).

You need to provision the following Azure services (preferably using Azure Government) - recommend putting all of these in the same 
Resource Group (Note: on our backlog is to provide an ARM template to automate the provisioning of these Azure resources):

* Azure Storage Account
* Azure Cosmos DB
* Azure AD App Registration
* Azure Web App
* Cognitive Services Account for Face API
* Cognitive Services Account for Computer Vision
* Cognitive Services Account for Translation
* Cognitive Services Account for Text Analytics (not yet available in Azure Government, use commercial Azure for this one)
* Cognitive Services Account for Speaker Recognition (not yet available in Azure Government, use commercial Azure for this one)

Before running the app, you will notice that the `appsettings.json` file contains many values specified as `"<enter-value>"`. We utilize the ASP.NET
Core [Secret Manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets) to prevent sensitive values from being checked into source control.

If using Visual Studio, right-click on the web project and select "Manage User Secrets" - fill in secrets.json with the appropriate values
from your provisioned resources in Azure:

```
{
  "Authentication:AzureAd:ClientId": "your-ad-app-registration-client-id",
  "Authentication:AzureAd:Domain": "your-ad-tenant-domain.onmicrosoft.com",
  "Authentication:AzureAd:TenantId": "your-ad-tenant-id",
  "IntelligentMissionConfig:Keys:TextTranslationKey": "your-text-translation-key",
  "IntelligentMissionConfig:Keys:TextAnalyticsKey": "your-text-analytics-key",
  "IntelligentMissionConfig:Keys:SpeakerRecognitionKey": "your-speaker-recognition-key",
  "IntelligentMissionConfig:Keys:FaceApiKey": "your-face-api-key",
  "IntelligentMissionConfig:Keys:ComputerVisionApiKey": "your-computer-vision-key",
  "IntelligentMissionConfig:Keys:VideoApiKey": "your-video-api-key",
  "IntelligentMissionConfig:StorageConfig:AccountName": "your-storage-account-name",
  "IntelligentMissionConfig:StorageConfig:AccountKey": "your-storage-account-key",
  "IntelligentMissionConfig:StorageConfig:EndpointSuffix": "core.usgovcloudapi.net",
  "IntelligentMissionConfig:DocDbConfig:EndpointUri": "your-cosmos-db-endpoint",
  "IntelligentMissionConfig:DocDbConfig:PrimaryKey": "your-cosmos-db-primary-key",
}
```

For simplicity, you can deploy to Azure by right-clicking Web project and selecting **"Publish"** to go through the standard Visual Studio Azure publish process.

Before running in Azure, make sure your have created Environment Variables (via App Settings in your Azure Web App) that correspond to all the User Secrets values.


# Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.microsoft.com.

When you submit a pull request, a CLA-bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., label, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
