using FluentValidation;
using Infrastructure.Behaviours;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Application.Infra
{
    public static class MediatrInternal
    {
        public static void AddMediatrInternal(this IServiceCollection services, params Type[] types)
        {
            //var assemblies = new List<Assembly>();
            //var handlers = new List<Type>();
            //foreach (var type in types)
            //{
            //    var assembly = type.Assembly;
            //    handlers.AddRange(assembly.GetTypes()
            //        .Where(t =>
            //            t.Namespace == type.Namespace
            //            &&
            //            t.GetInterfaces().Any(i =>
            //                i.IsGenericType
            //                &&
            //                (i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>) || i.GetGenericTypeDefinition() == typeof(IRequestHandler<>))
            //            )
            //        ).ToList());
            //    assemblies.Add(assembly);
            //}

            //foreach (var handlerType in handlers)
            //{
            //    var @interface = handlerType.GetInterfaces().FirstOrDefault();
            //    if (@interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))
            //    {
            //        var requestType = @interface.GetGenericArguments()[0];
            //        var resultType = @interface.GetGenericArguments()[1];
            //        var handlerGenericType = typeof(IRequestHandler<,>).MakeGenericType(requestType, resultType);
            //        services.AddTransient(handlerGenericType, handlerType);
            //    }
            //    else if (@interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IRequestHandler<>))
            //    {
            //        var requestType = @interface.GetGenericArguments()[0];
            //        var handlerGenericType = typeof(IRequestHandler<>).MakeGenericType(requestType);
            //        services.AddTransient(handlerGenericType, handlerType);
            //    }
            //}

            //services.AddScoped<IMediator, Mediator>();
            //services.AddValidatorsFromAssemblies(assemblies);
            //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehaviour<,>));

            var assemblyTest = Assembly.GetExecutingAssembly();

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(assemblyTest);
            });
            services.AddValidatorsFromAssembly(assemblyTest);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehaviour<,>));
        }
    }
}
