
using BaseInfo.Extensions;

namespace BaseInfo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            ServiceExtensions.Init(builder.Configuration);
            builder.Services.AddControllersInternal();
            builder.Services.AddAuthorizationInternal();
            builder.Services.AddApiVersioningInternal();
            builder.Services.AddSwaggerInternal();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
