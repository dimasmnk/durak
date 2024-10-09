using System.Reflection;
using Durak.Core.Common.Interfaces;

namespace Durak.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddServicesFromAllAssemblies(this IServiceCollection services)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (var assembly in assemblies)
        {
            var serviceTypes = assembly.GetExportedTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces()
                    .Any(i => i.IsInterface && typeof(IService).IsAssignableFrom(i)));

            foreach (var implementationType in serviceTypes)
            {
                var interfaceType = implementationType.GetInterfaces()
                    .FirstOrDefault(i => i.IsInterface && typeof(IService).IsAssignableFrom(i) && i.Name == $"I{implementationType.Name}");

                if (interfaceType != null)
                {
                    services.AddScoped(interfaceType, implementationType);
                }
            }
        }
    }
}