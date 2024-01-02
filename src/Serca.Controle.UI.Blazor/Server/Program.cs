using Microsoft.AspNetCore.ResponseCompression;
using Serca.AspNetCore.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Load kestrel configuration (for the development environment, it is only loaded if the profile explicitly declares it)
if (builder.Configuration["SERCA_USE_REEL_CERT"] == "true")
{
    builder.WebHost.UseKestrel(so => so.ConfigureEndpoints());
}


builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
