using GymMan.Services.CQRS.Commands;
using GymMan.Services.CQRS.Events;
using GymMan.Services.CQRS.Queries;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace GymMan.Services.CQRS
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCqrs(this IServiceCollection services)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic && a.FullName.StartsWith("GymMan.Services")).ToArray();

            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();

                foreach (var type in types)
                {
                    var interfaces = type.GetInterfaces();

                    foreach (var iface in interfaces)
                    {
                        if (!type.IsClass || type.IsAbstract) continue;

                        if (iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(ICommandHandler<>))
                        {
                            services.AddScoped(iface, type);
                        }

                        if (iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(IQueryHandler<,>))
                        {
                            services.AddScoped(iface, type);
                        }
                    }
                }
            }

            services.AddScoped<ICommandDispatcher, CommandDispatcher>();
            services.AddScoped<IQueryDispatcher, QueryDispatcher>();
            services.AddScoped<EventPlayerService>();

            return services;
        }
    }
}
