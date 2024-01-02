# Package Serca.Authentication

Fournit la couche d'authentification.

## Get started

### Authentification par web service

Configuration du webservice 

``` json

// appsettings.json
"BackendApiInitializationEndpoint": "https://api.bevor.fr/login/Account/Init",

``` 

Injection de dépendance


``` C#

using Serca.Authentication

builder.Services.AddAuthenticationsServices();
```
