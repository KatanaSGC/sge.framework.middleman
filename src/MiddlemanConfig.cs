using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace sge.framework.middleman;

public static class MiddlemanConfig
{
    public static IServiceCollection AddMiddlemanConfig(this IServiceCollection services)
    {
        services.AddTransient<IMiddlemanSender, MiddlemanSender>();
        
        var handlerInterfaceType = typeof(IMiddlemanHandler<,>);
        foreach (var implementationType in ((IEnumerable<Assembly>)AppDomain.CurrentDomain.GetAssemblies())
                 .SelectMany<Assembly, Type>((Func<Assembly, IEnumerable<Type>>)(a => (IEnumerable<Type>)a.GetTypes()))
                 .Where<Type>((Func<Type, bool>)(t =>
                     t.IsClass && !t.IsAbstract && ((IEnumerable<Type>)t.GetInterfaces()).Any<Type>(
                         (Func<Type, bool>)(i =>
                             i.IsGenericType && i.GetGenericTypeDefinition() == handlerInterfaceType)))).ToList<Type>())
        {
            foreach (var serviceType in ((IEnumerable<Type>)implementationType.GetInterfaces()).Where<Type>(
                         (Func<Type, bool>)(i =>
                             i.IsGenericType && i.GetGenericTypeDefinition() == handlerInterfaceType)))
                services.AddTransient(serviceType, implementationType);
        }

        var behaviorInterfaceType = typeof(IMiddlemanPipelineBehavior<,>);
        var behaviorTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract)
            .ToList();

        foreach (var behaviorType in behaviorTypes)
        {
            var interfaces = behaviorType.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == behaviorInterfaceType);

            foreach (var serviceType in interfaces)
            {
                if (behaviorType.IsGenericTypeDefinition)
                    services.AddTransient(behaviorInterfaceType, behaviorType);
                else
                    services.AddTransient(serviceType, behaviorType);
            }
        }

        return services;
    }
}