using Application.OptionPatternModel;
using Domain;
using Infrastructure;
using Infrastructure.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Security.Claims;


namespace BaseInfo.Extensions
{
    public static class ServiceExtensions
    {
        public static IConfiguration Configuration { get; private set; }
        public static AppSettingsOption OptionsData { get; private set; }

        public static void Init(IConfiguration config) => Configuration = config;

        public static void InitOption(this IServiceCollection services)
            => OptionsData = services.BuildServiceProvider()
               .GetService<IOptions<AppSettingsOption>>().Value;
        public static void AddMongoDb(this IServiceCollection services)
        {
            //services.AddScoped(_ =>
            //{
            //    return new MongoContext(OptionsData.Mongo.Server, OptionsData.Mongo.DbName);
            //});
        }

        public static void AddDbContextInternal(this IServiceCollection services)
        {
            services.AddDbContext<DarooDbContext>(option => { option.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")); });
        }



        public static void AddCorsInternal(this IServiceCollection services)
        {
            services.AddCors(option =>
            {
                option.AddPolicy(name: "CorsPolicySpecificOrigins",
                builder =>
                {
                    builder.SetIsOriginAllowedToAllowWildcardSubdomains()
                        .WithOrigins(OptionsData.AllowedOrigins.Split(','))
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
        }

        public static void AddControllersInternal(this IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson();
            //config.Filters.Add(typeof(ExceptionHandlerFilter));
        }

        public static void AddApiVersioningInternal(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
            });
            
        }

        public static void AddSwaggerInternal(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.EnableAnnotations();
                using (var serviceProvider = services.BuildServiceProvider())
                {
                    var provider = serviceProvider.GetRequiredService<IApiVersionDescriptionProvider>();
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerDoc(description.GroupName, new OpenApiInfo()
                        {
                            Title = $"{typeof(Program).Assembly.GetCustomAttribute<AssemblyProductAttribute>().Product} {description.ApiVersion}",
                            Version = description.ApiVersion.ToString()
                        });
                    }
                }

                options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "ApiKey"
                            },
                            In= ParameterLocation.Header,
                        },
                        Array.Empty<string>()
                    }
                });
            });
        }

        public static void AddAuthorizationInternal(this IServiceCollection services)
        {
            var authority = OptionsData.Settings.IdentitySettings.Authority;
            var cookieName = OptionsData.Settings.CookieInfo.Name;
            var requiredHttps = OptionsData.Settings.IdentitySettings.RequireHttpsMetadata;

            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.Authority = authority;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidIssuer = authority,
                    ValidTypes = ["at+jwt"],
                    ValidateIssuer = false
                };
                o.RequireHttpsMetadata = requiredHttps;

                o.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var token = context.Request.Headers[HeaderNames.Authorization].FirstOrDefault();
                        if (string.IsNullOrWhiteSpace(token))
                            token = context.Request.Cookies[cookieName];
                        else
                        {
                            token = token.Split(' ').LastOrDefault();
                            token = token.Split('=').LastOrDefault();
                        }

                        context.Token = token;
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        var claims = context.Principal.Identity as ClaimsIdentity;
                        if (!claims.Claims.Any())
                            context.Fail(Messages.Error401);

                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        if (context.AuthenticateFailure != null)
                        {
                            Console.WriteLine(context.ErrorDescription);
                            throw new Exception(Messages.Error401);
                        }

                        throw new Exception(Messages.Error401);
                    }
                };
            });
        }

        public static void AddOptionPatternInternal(this IServiceCollection services)
        {
            services.AddOptions<AppSettingsOption>()
                .Bind(Configuration);
            //.ValidateMiniValidation();
            services.InitOption();

            SetStaticValues();
        }

        private static void SetStaticValues()
        {
            ConstantValuesInfra.UsernameClaim = OptionsData.Settings.CookieInfo.UsernameClaim;
            ConstantValuesInfra.CookieName = OptionsData.Settings.CookieInfo.Name;
        }
    }
}
