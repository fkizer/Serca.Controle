using Microsoft.Extensions.FileProviders;

namespace Serca.Controle.UI.Blazor.Server.Extensions
{
    public static class UploadAppBuilderExtensions
    {
        public static IApplicationBuilder UseUploads(this IApplicationBuilder app)
        {
            var pathUploads = Path.Combine(Directory.GetCurrentDirectory(), @"Uploads");

            if (!Directory.Exists(pathUploads))
            {
                Directory.CreateDirectory(pathUploads);
            }

            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(pathUploads),
                RequestPath = new PathString("/Uploads")
            });

            return app;
        }
    }
}
