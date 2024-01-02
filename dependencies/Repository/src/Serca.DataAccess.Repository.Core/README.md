# Serca.DataAccess.Repository

Ce module est destiné à être consommé comme une librairie. Il sera donc déplacer dans un autre repository git.

Il fournis l'interface _IGenericRepository<TEntity, TKey>_ représentation de la couche d'accès, sur la base du _Repository Pattern_. Le concept est d'avoir une classe générique à laquelle on fournis le type de la ressource que l'on souhaite intérroger, ainsi que le type de sa clé. La classe fournis ensuite 5 opérations possible sur cette entité (CRUD) :

* Récupéré la liste de toutes les entités
* Récupéré une entité par son id
* Ajouter une entité (non implémenté)
* Mettre à jour une entité (non implémenté)
* Supprimer une entité (non implémenté)

Via cette interface, on laisse la possibilité sur l'implémentation de la manière dont c'est opération sont concrêtement réalisé, ce qui permet de connecter le repository à plusieur solution : base de données, web services, etc.

## Structure du module

``` bash

├───src
│   ├───Serca.DataAccess.Repository.Abstractions # Déclare les interfaces pour la couche infrastructure
│   │   ├───Cache # Déclare les interfaces pour la gestion du cache
│   ├───Serca.DataAccess.Repository.Core
│   └───Serca.DataAccess.Repository.WebServices # Fournis une implémentation de Serca.DataAccess.Repository.Abstractions
└───tests
    └───Serca.DataAccess.Repository.WebServices.Tests # Contient les tests unitaire et d'intégrations

```

## Get strated

``` C#
// Program.cs
// Dependency injection

builder.Services.AddOptions();
builder.Services.Configure<ApiRepositoryOptions>(configuration.GetSection("ApiRepositories"));
builder.Services.AddTransient(typeof(IGenericRepository<DeviceParametersEntity, int?>), typeof(GenericAPIRepository<DeviceParametersEntity, int?>));
```

``` json
// appsettings.json
// Configurating endpoints 
"ApiRepositories": {
    "Endpoints": [
        {
        "Resource": "Serca.PLI.Core.Domain.Entities.DeviceParametersEntity",
        "Url": "/api/pli/v1/parametre"
        },
        {
        "Resource": "Serca.PLI.Core.Domain.Entities.PreparateurEntity",
        "Url": "/api/pli/v1/preparateurs"
        }
    ]
}
```

``` C#
protected readonly GenericAPIRepository<DeviceParametersEntity, int?> Repository;

public ContextService(IGenericRepository<DeviceParametersEntity, int?> repository)
{
    Repository = (GenericAPIRepository<DeviceParametersEntity, int?>)repository;
}

public async Task UseRepository()
{
    Repository.SetParameters(GetDefaultWebServiceParameters());
    var deviceParamterEntity = await Repository.GetByIdAsync(null);
}
```

## Implémentation dans Serca.DataAccess.Repository.WebServices

Le projet propose une implémentation des interfaces fournis pas Serca.DataAccess.Repository.Abstractions. Elle utilise un client HTTP pour venir intéroger un web service afin de réalisé les opérations CRUD.

L'implémentation fournis également la possibilité de mettre en cache en mémoire (encore en phase de développement) pour réduire le volume des appels HTTP.

## Tests

### Serca.DataAccess.Repository.WebServices.Tests

La classe GenericAPIRepositoryTestIntegration permet de réaliser des tests d'intégration. Attention les tests sont actuellement liés avec des API propre à une application existante. Si les webservices évolue il se peut que les tests ne passent plus.
Elle founis également c'est propres entité pour la conversion des web services en objets pour ne pas dépendre d'un autre projet. Les tests peuvent être améliorés :

* La mise en cache n'est pas testé
