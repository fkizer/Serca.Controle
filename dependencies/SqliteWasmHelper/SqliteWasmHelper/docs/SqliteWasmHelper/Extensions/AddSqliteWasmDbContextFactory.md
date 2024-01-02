# Extensions.AddSqliteWasmDbContextFactory&lt;TContext&gt; method (1 of 2)

Add helper factory.
```csharp
public static IServiceCollection AddSqliteWasmDbContextFactory<TContext>(
    this IServiceCollection serviceCollection, 
    Action<DbContextOptionsBuilder>? optionsAction = null, 
    ServiceLifetime lifetime = ServiceLifetime.Singleton)
    where TContext : DbContext
```
| parameter | description |
| --- | --- |
| TContext | The DbContext being wrapped. |
| serviceCollection | The IServiceCollection. |
| optionsAction | An action used to configure DbContextOptions. |
| lifetime | Lifetime of the service. |
## Return Value
The service implementation.
## See Also
* class [Extensions](../Extensions.md)
* namespace [SqliteWasmHelper](../../SqliteWasmHelper.md)
---
# Extensions.AddSqliteWasmDbContextFactory&lt;TContext&gt; method (2 of 2)
    Action<IServiceProvider, DbContextOptionsBuilder>? optionsAction, 
<!-- DO NOT EDIT: generated by xmldocmd for SqliteWasmHelper.dll -->


Version 1.1.0-beta+9209ba6c71 generated on 04/21/2022 20:49:13.