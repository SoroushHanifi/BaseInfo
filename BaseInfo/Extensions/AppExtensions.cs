
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;

namespace BaseInfo.Extensions
{
    public static class AppExtensions
    {
        

        public static void UseSwaggerInternal(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var provider = scope.ServiceProvider.GetRequiredService<IApiVersionDescriptionProvider>();

            app.UseSwagger(c => c.PreSerializeFilters.Add((swagger, httpReq) =>
            {
                var referer = httpReq.Headers.Where(h => h.Key == "Referer")
                    .Select(r => r.Value.ToString().Replace("/swagger/index.html", string.Empty))
                    .FirstOrDefault();
                swagger.Servers = new List<OpenApiServer> { new() { Url = referer } };
            })).UseSwaggerUI(options =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                }
            });
        }

        public static void UseEndpointsInternal(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
