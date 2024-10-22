using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.AspNetCore.ResponseCompression;
using Serca.AspNetCore.Hosting;
using Serca.Authentication.Configuration;
using Serca.DataAccess.Models;
using Serca.DataAccess.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Reflection;
using System.Text;
using Hellang.Middleware.ProblemDetails;
using ProblemDetailsOptions = Hellang.Middleware.ProblemDetails.ProblemDetailsOptions;
using Serca.Controle.UI.Blazor.Server.Extensions;

var CorsPolicy = "_CorsPolicy";

var builder = WebApplication.CreateBuilder(args);

//Required for support dotnet user secrets in pretag-development environment
var env = builder.Environment;
if ((env.IsDevelopment() || env.EnvironmentName.ToLowerInvariant().Contains("development"))
    && !string.IsNullOrEmpty(env.ApplicationName))
{
    var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
    if (appAssembly != null)
    {
        builder.Configuration.AddUserSecrets(appAssembly, optional: true);
    }
}

//Required for support web asset in pretag-development environment
StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

// Load kestrel configuration (for the development environment, it is only loaded if the profile explicitly declares it)
if (builder.Configuration["SERCA_USE_REEL_CERT"] == "true")
{
    builder.WebHost.UseKestrel(so => so.ConfigureEndpoints());
}

// Add services to the container.
builder.Services.AddProblemDetails(ConfigureProblemDetails);
builder.Services.AddResponseCaching();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var apiClientsCredentials = new List<ApiClientCredential>();
builder.Configuration.Bind("ApiClientsCredentials:Clients", apiClientsCredentials);

var apiName = "pli";

builder.Services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
    .AddIdentityServerAuthentication(options =>
{
    options.Authority = builder.Configuration["IdentityServerURI"];
    options.ApiName = apiName;
    options.ApiSecret = apiClientsCredentials.FirstOrDefault(x => x.ClientId == apiName)?.ClientSecret;
    options.EnableCaching = true;
    options.CacheDuration = TimeSpan.FromMinutes(10);
});

builder.Services.AddHttpClient();

builder.Services.AddSingleton<GetTokenService>();

var corsAllowedOrigins = builder.Configuration.GetSection("cors_allowed_origins").Get<string[]>();

if (corsAllowedOrigins != null)
{
builder.Services.AddCors(options =>
{
options.AddPolicy(name: CorsPolicy,
    builder => builder
      .SetIsOriginAllowedToAllowWildcardSubdomains()
      .WithOrigins(corsAllowedOrigins)
      .AllowAnyMethod()
      .AllowCredentials()
      .AllowAnyHeader()
      .Build()
    );
});
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName.ToLowerInvariant().Contains("development"))
{
app.UseWebAssemblyDebugging();
}
else
{
app.UseExceptionHandler("/Error");
// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
app.UseHsts();
}

app.UseProblemDetails();
app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseUploads();

app.UseCors(CorsPolicy);
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.Map("/api", HandleMapApi);
app.Map("/204", Handle204);


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();


partial class Program
{


    private static void ConfigureProblemDetails(ProblemDetailsOptions options)
    {
        // Only include exception details in a development environment. There's really no nee
        // to set this as it's the default behavior. It's just included here for completeness :)
        // options.IncludeExceptionDetails = (ctx, ex) => environment.IsDevelopment();
        options.IncludeExceptionDetails = (ctx, ex) => {
            var env = ctx.RequestServices.GetRequiredService<IHostEnvironment>();
            return env.IsDevelopment() || env.IsStaging();
        };

        // You can configure the middleware to re-throw certain types of exceptions, all exceptions or based on a predicate.
        // This is useful if you have upstream middleware that needs to do additional handling of exceptions.
        options.Rethrow<NotSupportedException>();

        // This will map NotImplementedException to the 501 Not Implemented status code.
        options.MapToStatusCode<NotImplementedException>(StatusCodes.Status501NotImplemented);

        // This will map HttpRequestException to the 503 Service Unavailable status code.
        options.MapToStatusCode<HttpRequestException>(StatusCodes.Status503ServiceUnavailable);

        // Because exceptions are handled polymorphically, this will act as a "catch all" mapping, which is why it's added last.
        // If an exception other than NotImplementedException and HttpRequestException is thrown, this will handle it.
        options.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);
    }

    private static void HandleMapApi(IApplicationBuilder app)
    {
        app.Run(async (context) =>
        {
            var configuration = app.ApplicationServices.GetRequiredService<IConfiguration>();
            var bypassAuth = configuration.GetValue<bool>("BYPASS_AUTH");

            if (!bypassAuth && (!context.User.Identity?.IsAuthenticated ?? false))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }

            if (!context.Request.Query.ContainsKey("erp"))
            {
                throw new Exception("Erp params missing");
            }

            var erp = context.Request.Query["erp"];
            var parameters = context.Request.Query.Keys.Cast<string>().ToDictionary(k => k, v => context.Request.Query[v]);
            parameters.Remove("erp");

            var newQueryString = string.Join("&", parameters.Select(kvp => $"{kvp.Key}={kvp.Value}"));

            var httpClient = app.ApplicationServices.GetRequiredService<HttpClient>();

            var tokenService = app.ApplicationServices.GetRequiredService<GetTokenService>();

            string webserviceServerURI = configuration["WSUrl"];
            IdsrvToken token = null;
            token = await tokenService.GetToken(configuration, $"wmsserver{erp}");

            string requestURI = $"{context.Request.Path.Value}?{newQueryString}";
            var logger = app.ApplicationServices.GetService<ILogger<Program>>();
            logger?.LogInformation($"Request API : {requestURI}");


            Dictionary<string, HttpMethod> methods = new Dictionary<string, HttpMethod>()
                {
                    {"GET", HttpMethod.Get},
                    {"POST", HttpMethod.Post},
                    {"PUT", HttpMethod.Put},
                    {"DELETE", HttpMethod.Delete},
                    {"HEAD", HttpMethod.Head},
                    {"PATCH", HttpMethod.Patch},
                    {"OPTIONS", HttpMethod.Options}
                };
            HttpMethod requestMethod = methods[context.Request.Method.ToUpper()];

            StringContent requestContent = null;

            using (StreamReader reader = new StreamReader(context.Request.Body))
            {
                string text = await reader.ReadToEndAsync();
                requestContent = new StringContent(text, Encoding.UTF8, "application/json");
            }

            HttpRequestMessage request = new HttpRequestMessage(requestMethod, string.Concat(webserviceServerURI, requestURI));
            request.Headers.Add("AuthorizationId", $"Bearer {token.Token}");

            var cookieKey = "SERVER_API";
            foreach (var cookie in context.Request.Cookies)
            {
                if (cookie.Key == cookieKey)
                {
                    request.Headers.Add("Cookie", $"{cookie.Key}={cookie.Value}");
                }
            }

            request.Content = requestContent;
            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead);

            //TODO: Use Buffer or MemoryStream
            var content = await response.Content.ReadAsStringAsync();

            //Init with default value
            var contentType = "application/octet-stream";

            try
            {
                contentType = response.Content.Headers.GetValues("Content-Type").FirstOrDefault() ?? contentType;
            }
            catch (Exception)
            {
                //Blank
            }

            context.Response.ContentType = contentType;
            context.Response.StatusCode = (int)response.StatusCode;

            await context.Response.WriteAsync(content, Encoding.UTF8);
        });
    }

    private static void Handle204(IApplicationBuilder app)
    {
        app.Run(async (context) =>
        {
            context.Response.StatusCode = (int)HttpStatusCode.NoContent;
        });
    }
}