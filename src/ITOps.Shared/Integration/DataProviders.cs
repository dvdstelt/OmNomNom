using Microsoft.Extensions.DependencyInjection;

namespace ITOps.Shared.Integration;

public static class DataProviders
{
    public static void RegisterAll(IServiceCollection serviceCollection)
    {
        var assemblies = ReflectionHelper.GetAssemblies(".Data.dll");
        var types = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IProvideData).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        foreach (var type in types)
        {
            foreach (var @interface in type.GetDirectlyImplementedInterfaces())
            {
                serviceCollection.AddTransient(@interface, type);
            }
        }
    }

    static Type[] GetDirectlyImplementedInterfaces(this Type @type)
    {
        var allInterfaces = new HashSet<Type>(@type.GetInterfaces());
        var baseType = @type.BaseType;

        if (baseType != null)
        {
            allInterfaces.ExceptWith(baseType.GetInterfaces());
        }
        var toRemove = new HashSet<Type>();
        foreach (var implementedByMostDerivedClass in allInterfaces)
        {
            foreach (var implementedByOtherInterfaces in implementedByMostDerivedClass.GetInterfaces())
            {
                toRemove.Add(implementedByOtherInterfaces);
            }
        }

        allInterfaces.ExceptWith(toRemove);
        return allInterfaces.ToArray();
    }
}