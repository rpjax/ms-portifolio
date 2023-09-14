using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace ModularSystem.Core;

/// <summary>
/// A lightweight IoC container for registering and resolving dependencies. <br/>
/// This static class functions as an Inversion of Control (IoC) container allowing for Dependency Injection (DI).
/// <br/><br/>
/// Usage:<br/>
/// <see cref="Register{T}(T)"/>
/// <br/><br/>
/// <see cref="Get{T}"/>
/// <br/>
/// <see cref="GetInterface{T}"/>
/// </summary>
public static class DependencyContainer
{
    /// <summary>
    /// Internal concurrent dictionary to store dependencies safely across multiple threads.
    /// </summary>
    static readonly ConcurrentDictionary<string, Dependency> dependencies = new ConcurrentDictionary<string, Dependency>();

    /// <summary>
    /// Attempts to register a dependency without specifying its type or throwing exceptions.
    /// </summary>
    /// <param name="dependency">Instance of the dependency.</param>
    /// <returns>True if the registration was successful, false otherwise.</returns>
    public static bool TryRegister(object dependency)
    {
        var type = dependency.GetType();

        if (Contains(type))
        {
            return false;
        }

        dependencies[type.FullName!] = CreateDependency(type, dependency);
        return true;
    }

    /// <summary>
    /// Registers a dependency without specifying its type.
    /// </summary>
    /// <param name="dependency">Instance of the dependency.</param>
    /// <exception cref="InvalidOperationException">Thrown if the type is already registered.</exception>
    public static void Register(object dependency)
    {
        if (!TryRegister(dependency))
        {
            throw new InvalidOperationException("Dependency container exception, type already registered.");
        }
    }

    /// <summary>
    /// Attempts to register a dependency without throwing exceptions.
    /// </summary>
    /// <typeparam name="T">Type of dependency to register.</typeparam>
    /// <param name="dependency">Instance of the dependency.</param>
    /// <returns>True if the registration was successful, false otherwise.</returns>
    public static bool TryRegister<T>(T dependency) where T : class
    {
        var type = typeof(T);

        if (Contains<T>())
        {
            return false;
        }

        dependencies[type.FullName!] = CreateDependency(dependency.GetType(), dependency);
        return true;
    }

    /// <summary>
    /// Registers a dependency for a specific type.
    /// </summary>
    /// <typeparam name="T">Type of dependency to register.</typeparam>
    /// <param name="dependency">Instance of the dependency.</param>
    /// <exception cref="InvalidOperationException">Thrown if the type is already registered.</exception>
    public static void Register<T>(T dependency) where T : class
    {
        if (!TryRegister<T>(dependency))
        {
            throw new InvalidOperationException("Dependency container exception, type already registered.");
        }
    }

    public static bool Contains(Type type)
    {
        return
            dependencies.ContainsKey(type.FullName!) ||
            dependencies.Values
                .Where(x => x.TypeName == type.FullName)
                .IsNotEmpty();
    }

    public static bool ContainsInterface(Type type)
    {
        return dependencies
            .Where(x => x.Value.Implements(type))
            .IsNotEmpty();
    }

    public static bool Contains<T>()
    {
        return Contains(typeof(T));
    }

    public static bool ContainsInterface<T>()
    {
        return ContainsInterface(typeof(T));
    }

    public static object? TryGet(Type type)
    {
        if (type.FullName == null)
        {
            throw new ArgumentException(nameof(type));
        }

        if (dependencies.ContainsKey(type.FullName))
        {
            return dependencies[type.FullName].Object;
        }

        var query = dependencies.Values
            .Where(x => x.TypeName == type.FullName);

        if (query.IsNotEmpty())
        {
            return query.First().Object;
        }

        return null;
    }

    public static object? TryGetInterface(Type type)
    {
        var dep = dependencies
            .Where(x => x.Value.Implements(type))
            .Select(x => x.Value.Object)
            .FirstOrDefault();
        return dep;
    }

    public static T? TryGet<T>() where T : class
    {
        return TryGet(typeof(T))
            ?.TryTypeCast<T>();
    }

    public static T? TryGetInterface<T>() where T : class
    {
        return TryGetInterface(typeof(T))
            ?.TryTypeCast<T>();
    }

    public static bool TryGet<T>([NotNullWhen(true)] out T? dependency) where T : class
    {
        dependency = TryGet(typeof(T))
            ?.TypeCast<T>();

        return dependency != null;
    }

    public static bool TryGetInterface<T>([NotNullWhen(true)] out T? dependency) where T : class
    {
        dependency = TryGetInterface(typeof(T))
            ?.TypeCast<T>();

        return dependency != null;
    }

    public static T Get<T>() where T : class
    {
        var dependency = TryGet<T>();

        if (dependency == null)
        {
            throw new InvalidOperationException("Dependency container exception, tried to get a type that has not been registered.");
        }

        return dependency;
    }

    public static T GetInterface<T>() where T : class
    {
        var dependency = TryGetInterface<T>();

        if (dependency == null)
        {
            throw new InvalidOperationException("Dependency container exception, tried to get a type that has not been registered.");
        }

        return dependency;
    }

    /// <summary>
    /// Helper method to create a new dependency instance.
    /// </summary>
    /// <param name="type">Type of the dependency.</param>
    /// <param name="value">Instance of the dependency.</param>
    /// <returns>A new <see cref="Dependency"/> instance.</returns>
    static Dependency CreateDependency(Type type, object value)
    {
        var name = type.FullName!;
        var interfaces = type.GetInterfaces();
        var dependency = new Dependency(name, interfaces.Select(x => x.FullName!).ToArray(), value);
        return dependency;
    }

    /// <summary>
    /// Internal representation of a dependency, containing details about its type and implemented interfaces.
    /// </summary>
    internal class Dependency
    {
        public string TypeName { get; set; }
        public string[] ImplementedInterfaces { get; set; } = new string[0];
        public object Object { get; set; }

        public Dependency(string name, string[] implementedInterfaces, object value)
        {
            TypeName = name;
            ImplementedInterfaces = implementedInterfaces;
            Object = value;
        }

        public bool Implements(Type interfaceType)
        {
            return ImplementedInterfaces.Any(x => x == interfaceType.FullName);
        }

        public bool Implements<T>()
        {
            return Implements(typeof(T));
        }
    }
}

public static class DependencyContainerExtensions
{

}