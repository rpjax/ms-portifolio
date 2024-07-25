using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace ModularSystem.Core;

/// <summary>
/// A lightweight IoC container for registering and resolving dependencies.<br/>
/// This class functions as an Inversion of Control (IoC) container allowing for Dependency Injection (DI).
/// </summary>
public class DependencyContainerObject
{
    /// <summary>
    /// The underlying storage for dependencies. The key is the full name of the type.
    /// </summary>
    private readonly ConcurrentDictionary<string, Dependency> dependencies = new ConcurrentDictionary<string, Dependency>();

    /// <summary>
    /// Attempts to register a dependency of a specific type.<br/>
    /// This method uses the type's full name as the key, ensuring a unique identifier for each registered dependency.
    /// </summary>
    /// <param name="type">The type of the dependency to register.</param>
    /// <param name="dependency">The object instance to register.</param>
    /// <returns>True if the registration was successful, otherwise false.</returns>
    public bool TryRegister(Type type, object dependency)
    {
        if (Contains(type))
        {
            return false;
        }

        dependencies[CreateKey(type)] = CreateDependency(dependency.GetType(), dependency);
        return true;
    }

    /// <summary>
    /// Attempts to register a dependency using <typeparamref name="T"/> as key.
    /// </summary>
    /// <typeparam name="T">Type of dependency to register.</typeparam>
    /// <param name="dependency">Instance of the dependency.</param>
    /// <returns>True if the registration was successful, false otherwise.</returns>
    public bool TryRegister<T>(T dependency) where T : class
    {
        return TryRegister(typeof(T), dependency);
    }

    /// <summary>
    /// Registers a dependency for a specific type.
    /// </summary>
    /// <typeparam name="T">Type of dependency to register.</typeparam>
    /// <param name="dependency">Instance of the dependency.</param>
    /// <exception cref="InvalidOperationException">Thrown if the type is already registered.</exception>
    public void Register<T>(T dependency) where T : class
    {
        if (!TryRegister(dependency))
        {
            throw new InvalidOperationException("Dependency container exception, type already registered.");
        }
    }

    /// <summary>
    /// Checks if a specific type is registered in the container.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>True if the type is registered, otherwise false.</returns>
    public bool Contains(Type type)
    {
        var key = CreateKey(type);

        if (dependencies.ContainsKey(key))
        {
            return true;
        }

        foreach (var dependency in dependencies.Values)
        {
            if (dependency.TypeFullName == type.FullName)
            {
                return true;
            }
            if (dependency.Implements(type))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Checks if a specific type is registered in the container.
    /// </summary>
    /// <typeparam name="T">The type to check.</typeparam>
    /// <returns>True if the type is registered, otherwise false.</returns>
    public bool Contains<T>()
    {
        return Contains(typeof(T));
    }

    /// <summary>
    /// Attempts to retrieve a registered dependency of a specific type.
    /// </summary>
    /// <param name="type">The type of the dependency to retrieve.</param>
    /// <returns>The registered dependency if found, otherwise null.</returns>
    public object? TryGet(Type type)
    {
        var key = CreateKey(type);

        if (dependencies.ContainsKey(key))
        {
            return dependencies[key].Object;
        }

        foreach (var dependency in dependencies.Values)
        {
            if (dependency.TypeFullName == type.FullName)
            {
                return dependency.Object;
            }
            if (dependency.Implements(type))
            {
                return dependency.Object;
            }
        }

        return null;
    }

    /// <summary>
    /// Attempts to retrieve a registered dependency of a specific type.
    /// </summary>
    /// <typeparam name="T">The type of the dependency to retrieve.</typeparam>
    /// <returns>The registered dependency if found, otherwise null.</returns>
    public T? TryGet<T>() where T : class
    {
        return TryGet(typeof(T))?.TryTypeCast<T>();
    }

    /// <summary>
    /// Attempts to retrieve a registered dependency of a specific type.
    /// </summary>
    /// <typeparam name="T">The type of the dependency to retrieve.</typeparam>
    /// <param name="dependency">The retrieved dependency if found.</param>
    /// <returns>True if the dependency was found, otherwise false.</returns>
    public bool TryGet<T>([NotNullWhen(true)] out T? dependency) where T : class
    {
        dependency = TryGet(typeof(T))?.TypeCast<T>();
        return dependency != null;
    }

    /// <summary>
    /// Retrieves a registered dependency of a specific type.
    /// </summary>
    /// <param name="type">The type of the dependency to retrieve.</param>
    /// <returns>The registered dependency.</returns>
    /// <exception cref="Exception">Thrown if the type is not registered.</exception>
    public object Get(Type type)
    {
        var obj = TryGet(type);

        if (obj == null)
        {
            throw new Exception("Dependency container exception, tried to get a type that has not been registered.");
        }

        return obj;
    }

    /// <summary>
    /// Retrieves a registered dependency of a specific type.
    /// </summary>
    /// <typeparam name="T">The type of the dependency to retrieve.</typeparam>
    /// <returns>The registered dependency.</returns>
    public T Get<T>()
    {
        return Get(typeof(T)).TypeCast<T>();
    }

    /// <summary>
    /// Generates a unique key for the dependency based on the type's full name.<br/>
    /// This key is used to ensure that each registered dependency can be uniquely identified and retrieved.
    /// </summary>
    /// <param name="type">The type for which the key is to be generated.</param>
    /// <returns>A string representing the unique key for the dependency.</returns>
    /// <exception cref="ArgumentException">Thrown if the provided type does not have a full name.</exception>
    protected virtual string CreateKey(Type type)
    {
        if (type.FullName == null)
        {
            throw new ArgumentException($"Invalid type used as key in dependency container operation, the provided type '{type.Name}' does not have a fullname, witch is required.");
        }

        return type.FullName;
    }

    /// <summary>
    /// Helper method to create a new dependency instance.
    /// </summary>
    /// <param name="type">Type of the dependency.</param>
    /// <param name="value">Instance of the dependency.</param>
    /// <returns>A new <see cref="Dependency"/> instance.</returns>
    protected virtual Dependency CreateDependency(Type type, object value)
    {
        var name = type.FullName!;
        var interfaces = type.GetInterfaces();
        var dependency = new Dependency(name, interfaces.Select(x => x.FullName!).ToArray(), value);
        return dependency;
    }

    /// <summary>
    /// Represents a registered dependency, encapsulating details about its type and any interfaces it implements.
    /// This internal representation ensures that dependencies can be efficiently checked for compatibility with requested types or interfaces.
    /// </summary>
    protected internal class Dependency
    {
        public string TypeFullName { get; set; }
        public string[] ImplementedInterfaces { get; set; } = new string[0];
        public object Object { get; set; }

        public Dependency(string name, string[] implementedInterfaces, object value)
        {
            TypeFullName = name;
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

// The DependencyContainerExtensions class remains unchanged.
public static class DependencyContainerObjectExtensions
{

}
