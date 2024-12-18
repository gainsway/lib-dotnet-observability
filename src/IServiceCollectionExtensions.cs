using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Gainsway.Observability;

public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds tracing decorators to the specified services in the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the tracing decorators to.</param>
    /// <param name="traceableServices">A list of service types to be decorated with tracing functionality.</param>
    /// <returns>The <see cref="IServiceCollection"/> with the tracing decorators added.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the 'Create' method is not found on the 'TraceDecorator' type for a given service.
    /// </exception>
    public static IServiceCollection TraceServices(
        this IServiceCollection services,
        IList<Type> traceableServices
    )
    {
        foreach (var serviceToBeTraced in traceableServices)
        {
            services.Decorate(
                serviceToBeTraced,
                (decorated) =>
                    typeof(TraceDecorator<>)
                        .MakeGenericType(serviceToBeTraced)
                        .GetMethod("Create")
                        ?.Invoke(null, [decorated])
                    ?? throw new InvalidOperationException(
                        $"Method 'Create' not found on type 'TraceDecorator<{serviceToBeTraced.Name}>'"
                    )
            );
        }

        return services;
    }

    /// <summary>
    /// Adds tracing decorators to all services in the <see cref="IServiceCollection"/> that are interfaces
    /// and belong to the namespace.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the tracing decorators will be added.</param>
    /// <param name="_namespace">The namespace to which the services belong.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> with tracing decorators added.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the 'Create' method is not found on the 'TraceDecorator' type for a given service.
    /// </exception>
    public static IServiceCollection TraceAllServicesInNamespace(
        this IServiceCollection services,
        string _namespace = "Gainsway"
    )
    {
        var traceableServices = AppDomain
            .CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type =>
                type.IsInterface
                && services.Any(service => service.ServiceType == type)
                && type.Namespace?.StartsWith(_namespace) == true
            )
            .ToList();

        foreach (var serviceToBeTraced in traceableServices)
        {
            services.Decorate(
                serviceToBeTraced,
                (decorated) =>
                    typeof(TraceDecorator<>)
                        .MakeGenericType(serviceToBeTraced)
                        .GetMethod("Create")
                        ?.Invoke(null, [decorated])
                    ?? throw new InvalidOperationException(
                        $"Method 'Create' not found on type 'TraceDecorator<{serviceToBeTraced.Name}>'"
                    )
            );
        }

        return services;
    }

    public static IServiceCollection TraceDecoratedServices(this IServiceCollection services)
    {
        var traceableServices = AppDomain
            .CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type =>
                type.IsInterface
                && services.Any(service => service.ServiceType == type)
                && type.GetCustomAttributes<TraceableAttribute>().Any()
            )
            .ToList();

        foreach (var serviceToBeTraced in traceableServices)
        {
            services.Decorate(
                serviceToBeTraced,
                (decorated) =>
                    typeof(TraceDecorator<>)
                        .MakeGenericType(serviceToBeTraced)
                        .GetMethod("Create")
                        ?.Invoke(null, [decorated])
                    ?? throw new InvalidOperationException(
                        $"Method 'Create' not found on type 'TraceDecorator<{serviceToBeTraced.Name}>'"
                    )
            );
        }

        return services;
    }
}
