using MediatR;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using NLog.Config;
using NLog.Targets;
using NLog;
using Serca.Controle.Core.Application.Abstraction.Services;
using Serca.Controle.Core.Application.Constants;
using Serca.Controle.Core.Application.Data;
using Serca.Controle.Core.Application.Interfaces;
using Serca.Controle.Core.Application.UseCases.DeviceParametersUseCases;
using Serca.Controle.Infrastructures.Logging;
using Serca.Controle.UI.Blazor.Client;
using Serca.Controle.UI.Blazor.Client.Managers;
using Serca.Controle.UI.Blazor.Client.Services;
using Serca.DataAccess.Abstractions.CacheManager;
using Serca.DataAccess.CacheManager;
using Serca.Tools.Barecode;
using Serca.Controle.UI.Blazor.Client.wwwroot;
using NLog.Extensions.Logging;
using Blazored.LocalStorage;
using Serca.Controle.UI.Blazor.Client.Extensions;
using Serca.Controle.Core.Application.ViewModels;
using SqliteWasmHelper;
using Microsoft.EntityFrameworkCore;
using Serca.Tools.Extensions;

#if BENCHMARK

var sw = Stopwatch.StartNew();

#endif

var builder = WebAssemblyHostBuilder.CreateDefault(args);

var app = new WebAssemblyHost[1];
var defaultHttpClientKey = "default";

// Init with default values
var baseAddress = builder.HostEnvironment.BaseAddress;
var backend_uri = baseAddress;
var idsrv_uri = builder.Configuration["IdentityServerURI"];

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.RootComponents.Add<Head>("head::after");

#if BENCHMARK

sw.Stop();
Console.WriteLine($"[BENCHMARK] Builder creation and adding root components : {sw.ElapsedMilliseconds}ms");
sw.Restart();

#endif

//Enabling logging configuration

builder.Logging
    .ClearProviders()
    .AddNLog(builder.Configuration);

// Adding oidc
builder.Services.AddOptions();
builder.Services.AddOidcAuthentication(options =>
{
    options.ProviderOptions.Authority = idsrv_uri;
    options.ProviderOptions.ClientId = "***";
    options.ProviderOptions.ResponseType = "code";
    options.ProviderOptions.DefaultScopes.Add("***");
    options.ProviderOptions.DefaultScopes.Add("depo");
    options.ProviderOptions.DefaultScopes.Add("logistics");
    options.ProviderOptions.DefaultScopes.Add("offline_access");
});

// App stack
builder.AddAuthenticationsServices(builder.Configuration);
builder.Services.AddScoped<AuthManager>();
builder.Services.AddScoped<AppBarManager>();
builder.Services.AddScoped<CookiesManager>();
builder.Services.AddScoped<NavigationManagerExtended>();
builder.Services.AddScoped<NotificationManager>();
builder.Services.AddScoped<UpdateManager>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddContextServices();
builder.Services.AddSqliteWasmDbContextFactory<ApplicationDbContext>(
  opts => opts.UseSqlite("Data Source=app.sqlite3"));
builder.Services.AddSingleton<IScanManager, ScanManager>();

//MudBlazor
builder.Services.AddMudServices();
builder.Services.AddScoped<LayoutService>();

//
builder.Services.AddCacheManager(builder.Configuration);
builder.Services.AddRepositories(builder.Configuration);


//MediatR & AutoMapper 
builder.Services.AddApplicationServices();

// Adding http client
builder.Services.AddScoped<CustomAuthorizationMessageHandler>();
builder.Services.AddSingleton<NetworkConfiguration>();
builder.Services.AddScoped<DiagnosticService>();

builder.Services.AddHttpClient(defaultHttpClientKey, httpClient =>
{
httpClient.BaseAddress = new Uri(backend_uri);
}).AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

builder.Services.AddScoped(sp =>
sp.GetRequiredService<IHttpClientFactory>().CreateClient(defaultHttpClientKey));

builder.Services.AddHttpClient(ApplicationConstants.DiagHttpClientName, httpClient => { httpClient.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress); httpClient.Timeout = TimeSpan.FromSeconds(5); });

//NLog
builder.Services.AddNLogCustomTargets(builder.Configuration);




#if BENCHMARK

sw.Stop();
Console.WriteLine($"[BENCHMARK] Builder services registration : {sw.ElapsedMilliseconds}ms");
sw.Restart();
#endif

app[0] = builder.Build();

#if BENCHMARK

sw.Stop();
Console.WriteLine($"[BENCHMARK] Build duration: {sw.ElapsedMilliseconds}ms");
sw.Restart();

#endif

if (builder.Configuration.GetValue<bool>("IsSerigInstance"))
{
var localStorage = app[0].Services.GetRequiredService<ISyncLocalStorageService>();
var vm = localStorage.GetItem<SerigProfileViewModel>("SerigProfile");

// "/" used for prevent error on invalide or empty value
var url = !String.IsNullOrEmpty(vm?.ServerURL) ? $"{vm.ServerURL}/" : "/";
backend_uri = url;
idsrv_uri = url;
}

//Save the configuration to share with CustomAuthorizationMessageHandler
var configure = app[0].Services.GetRequiredService<NetworkConfiguration>();
configure.BackendServerUrl = backend_uri;
configure.AttachAuthorizationMessageHandler(configure.BackendServerUrl);
configure.IdentityServerUrl = idsrv_uri;

if (builder.Configuration.GetValue<bool>("IsSerigInstance"))
{
configure.AttachAuthorizationMessageHandler(configure.IdentityServerUrl);
}

// Enabling NLog Target dependency injection
var builtInProvider = ConfigurationItemFactory.Default.CreateInstance;
ConfigurationItemFactory.Default.CreateInstance = type =>
{
return app[0].Services.GetService(type) ?? builtInProvider(type);
};


var ctx = app[0].Services.GetRequiredService(typeof(IContextService)) as IContextService;
var authStateProvider = app[0].Services.GetRequiredService(typeof(AuthenticationStateProvider)) as AuthenticationStateProvider;

//Ensure auth is load
await authStateProvider!.GetAuthenticationStateAsync();
//Then run context validation in background
_ = ctx!.ValidateAsync();


var mediator = app[0].Services.GetRequiredService(typeof(IMediator)) as IMediator;

//Gestion pilote
_ = Task.Run(async () =>
{
var result = await mediator!.Send(new GetDeviceParametersQuery() { GetFromCache = true });
if (result.Success && result.Data != null)
{
var cookieManager = app[0].Services.GetRequiredService(typeof(CookiesManager)) as CookiesManager;
var navigationManager = app[0].Services.GetRequiredService(typeof(NavigationManager)) as NavigationManager;

var isPilote = "false";
if (result.Data.IsPilote)
{
isPilote = "true";
}
await cookieManager!.CreateCookie("PLI_PILOTE", isPilote, 999999999, new Uri(builder.HostEnvironment.BaseAddress).Host);
}
});

#if BENCHMARK

sw.Stop();
Console.WriteLine($"[BENCHMARK] NLOG build duration: {sw.ElapsedMilliseconds}ms");

#endif

await app[0].RunAsync();


public partial class Program
{
    private static async Task InitializeCacheManagerState(IServiceProvider services)
    {
        var cacheManager = services.GetRequiredService<ICacheManager>();
        var cacheManagerState = services.GetRequiredService<CacheManagerState>();
        var localStorageService = services.GetRequiredService<IStorageService>();

        // Not really use module
        cacheManagerState.ActiveModule = cacheManager.GetModuleByName(ApplicationConstants.ApplicationName);

        var latestUpdatesWithBadKey = await localStorageService.GetAsync<Dictionary<string, DateTime?>>("LastUpdate");
        cacheManagerState.LatestUpdates = new Dictionary<string, DateTime?>();
        if (latestUpdatesWithBadKey != null)
        {
            foreach (var lastUpdate in latestUpdatesWithBadKey)
            {
                cacheManagerState.LatestUpdates.Add(lastUpdate.Key.FirstCharToUpper(), lastUpdate.Value);
            }
        }

        cacheManagerState.CacheManagerUpdate += async (sender, arg) =>
        {
            await localStorageService.SaveAsync(cacheManagerState.LatestUpdates, "LastUpdate");
        };
    }
}