
using System.Diagnostics;
using Asp.Versioning;
using BaseInfo.Extensions;
using Refit;
using Serilog;


namespace BaseInfo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Attach debugger when run from cli: dotnet run command
            if (args.Contains("--AttachDebugger") || args.Contains("AttachDebugger") || args.Contains("-- AttachDebugger"))
                Debugger.Launch();

            var builder = WebApplication.CreateBuilder(args);
            CreateLogging(builder);
            var services = builder.Services;
            

            ServiceExtensions.Init(builder.Configuration);
            services.InjectedClasses();
            services.AddRefitInternal();
            services.AddControllersInternal();
            services.AddAuthorizationInternal();
            services.AddApiVersioningInternal();
            services.AddSwaggerInternal();

            var app = builder.Build();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.UseSwaggerInternal(builder.Environment);
            app.UseEndpointsInternal(builder.Environment);

            app.Run();
        }

        public static void CreateLogging(WebApplicationBuilder builder)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .WriteTo.File(
                     path: $"{builder.Environment.ContentRootPath}/LogFiles/log.txt",
                     fileSizeLimitBytes: 5_000_000,
                     rollingInterval: RollingInterval.Day
                ).CreateLogger();
        }
    }
}
