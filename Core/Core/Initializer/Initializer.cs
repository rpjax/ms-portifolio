using ModularSystem.Core.Logging;
using System.Reflection;

namespace ModularSystem.Core;

/// <summary>
/// Represents a base class for custom initialization logic. <br/>
/// The sequence of the initialization pipeline is: BeforeInit -> InternalInit -> Init -> AfterInit.<br/>
/// Initializers are grouped by their priority and executed concurrently within their respective priority group.<br/>
/// To ensure a specific order of initialization, prioritize one initializer over another or override a subsequent hook method in the pipeline.
/// </summary>
public abstract class Initializer
{
    /// <summary>
    /// Gets the priority level for the current initializer. <br/>
    /// Higher priority values indicate that the initializer will be executed earlier in the sequence.
    /// </summary>
    public int Priority { get; protected set; } = (int)PriorityLevel.Normal;

    /// <summary>
    /// Invokes all <see cref="Initializer"/> within the current AppDomain.
    /// </summary>
    /// <param name="options">Custom options for the initialization process.</param>
    public static void Run(Options? options = null)
    {
        Run(AppDomain.CurrentDomain.GetAssemblies(), options);
    }

    /// <summary>
    /// Invokes all <see cref="Initializer"/> within the specified assembly.
    /// </summary>
    /// <param name="assembly">The assembly to search in.</param>
    /// <param name="options">Custom options for the initialization process.</param>
    public static void Run(Assembly assembly, Options? options = null)
    {
        Run(new Assembly[] { assembly }, options);
    }

    /// <summary>
    /// Invokes all <see cref="Initializer"/> within the given assembly array. <br/>
    /// Custom initializer classes override the <see cref="OnInitAsync"/> method to define their initialization logic.
    /// </summary>
    /// <param name="assemblies">The assemblies to search in.</param>
    /// <param name="options">Custom options for the initialization process.</param>
    public static void Run(Assembly[] assemblies, Options? options = null)
    {
        options ??= new Options();
        options.Assemblies = assemblies.ToList();

        var initializers = GetInitializersFrom(assemblies)
            .OrderByDescending(x => x.Priority)
            .ToList();

        if (options.EnableInitializationLogs)
        {
            Console.WriteLine("Starting application initialization...");
        }

        InvokeBeforeInit(initializers, options);
        InvokeInternalInit(initializers, options);
        InvokeOnInit(initializers, options);
        InvokeAfterInit(initializers, options);

        if (options.EnableInitializationLogs)
        {
            ConsoleLogger.Info("Application successfully initialized.");
        }
    }

    /// <summary>
    /// Contains logic that runs before the main initialization process. 
    /// This method runs first in the initialization pipeline.
    /// </summary>
    /// <param name="options">Configuration options for the initialization process.</param>
    protected virtual Task BeforeInitAsync(Options options)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Used by the library to initialize it's internal components. A hook to for before this can be created by overriding <see cref="BeforeInitAsync(Options)"/>.
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    protected internal virtual Task InternalInitAsync(Options options)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Contains the logic that needs to be executed during initialization. <br/>
    /// Derived classes must override this method to define their custom initialization behavior.
    /// </summary>
    protected virtual Task OnInitAsync(Options options)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Contains logic that runs after the main initialization process. 
    /// This method runs last in the initialization pipeline.
    /// </summary>
    /// <param name="options">Configuration options for the initialization process.</param>
    protected virtual Task AfterInitAsync(Options options)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Retrieves initializers from the specified assemblies.
    /// </summary>
    /// <param name="assemblies">Assemblies to search for initializers.</param>
    /// <returns>A list of initializers.</returns>
    static List<Initializer> GetInitializersFrom(Assembly[] assemblies)
    {
        return assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsSubclassOf(typeof(Initializer)))
            .Select(type => Activator.CreateInstance(type) as Initializer)
            .Where(x => x != null)
            .Select(x => x!)
            .ToList();
    }

    /// <summary>
    /// Invokes the <see cref="BeforeInitAsync(Options)"/> method on all provided initializers.
    /// </summary>
    /// <param name="initializers">A list of initializers to invoke.</param>
    /// <param name="options"></param>
    static void InvokeBeforeInit(List<Initializer>? initializers, Options options)
    {
        if (initializers == null) return;

        foreach (var group in initializers.GroupBy(x => x.Priority))
        {
            var tasks = new List<Task>();

            foreach (var initializer in group)
            {
                tasks.Add(initializer.BeforeInitAsync(options));
            }

            Task.WaitAll(tasks.ToArray());
        }
    }

    static void InvokeInternalInit(List<Initializer>? initializers, Options options)
    {
        if (initializers == null) return;

        foreach (var group in initializers.GroupBy(x => x.Priority))
        {
            var tasks = new List<Task>();

            foreach (var initializer in group)
            {
                tasks.Add(initializer.InternalInitAsync(options));
            }

            Task.WaitAll(tasks.ToArray());
        }
    }

    /// <summary>
    /// Invokes the <see cref="OnInitAsync(Options)"/> method on all provided initializers.
    /// </summary>
    /// <param name="initializers">A list of initializers to invoke.</param>
    /// <param name="options"></param>
    static void InvokeOnInit(List<Initializer>? initializers, Options options)
    {
        if (initializers == null) return;

        foreach (var group in initializers.GroupBy(x => x.Priority))
        {
            var tasks = new List<Task>();

            foreach (var initializer in group)
            {
                tasks.Add(initializer.OnInitAsync(options));
            }

            Task.WaitAll(tasks.ToArray());
        }
    }

    /// <summary>
    /// Invokes the <see cref="AfterInitAsync(Options)"/> method on all provided initializers.
    /// </summary>
    /// <param name="initializers">A list of initializers to invoke.</param>
    /// <param name="options"></param>
    static void InvokeAfterInit(List<Initializer>? initializers, Options options)
    {
        if (initializers == null) return;

        foreach (var group in initializers.GroupBy(x => x.Priority))
        {
            var tasks = new List<Task>();

            foreach (var initializer in group)
            {
                tasks.Add(initializer.AfterInitAsync(options));
            }

            Task.WaitAll(tasks.ToArray());
        }
    }

    /// <summary>
    /// Sets the priority of the current initializer.
    /// </summary>
    /// <param name="priorityLevel">The desired priority level.</param>
    protected void SetPriority(PriorityLevel priorityLevel)
    {
        Priority = (int)priorityLevel;
    }

    /// <summary>
    /// Represents custom options for controlling the initialization process.
    /// </summary>
    public class Options
    {
        /// <summary>
        /// Gets or sets custom initialization flags.
        /// </summary>
        public List<string> Flags { get; set; } = new();

        /// <summary>
        /// Gets or sets the assemblies that will be considered during the initialization process.
        /// </summary>
        public List<Assembly> Assemblies { get; set; } = new();

        /// <summary>
        /// Gets or sets a value indicating whether to look for initializers in the assemblies loaded in the current AppDomain.
        /// </summary>
        public bool LookForInitializersInAppDomainAssembly { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to show initialization messages.
        /// </summary>
        public bool EnableInitializationLogs { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to initialize the console logger.
        /// </summary>
        public bool InitConsoleLogger { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether disk logging is enabled for exceptions.
        /// When set to true, exceptions are logged to disk using the <see cref="ExceptionLogger.EnableDiskLogging"/> mechanism.
        /// </summary>
        public bool EnableDiskExceptionLogger { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether console logging is enabled for exceptions.
        /// When set to true (default), exceptions are logged to the console using the <see cref="ExceptionLogger.EnableConsoleLogging"/> mechanism.
        /// </summary>
        public bool EnableConsoleExceptionLogger { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to initialize entity configurations during the application startup.
        /// </summary>
        public bool InitializeEntityConfigurations { get; set; } = false;

        /// <summary>
        /// Gets or sets the JSON serialization options for the application.
        /// </summary>
        public JsonSerializationOptions JsonSerialization { get; set; } = new();

        /// <summary>
        /// Determines if a specific flag exists in the Flags list.
        /// </summary>
        /// <param name="flag">The flag to check for.</param>
        /// <returns>True if the flag exists, false otherwise.</returns>
        public bool ContainsFlag(string flag)
        {
            return Flags.Contains(flag);
        }

        /// <summary>
        /// Adds a new flag to the Flags list if it doesn't already exist.
        /// </summary>
        /// <param name="flag">The flag to add.</param>
        /// <returns>The current instance of Options.</returns>
        public Options AddFlag(string flag)
        {
            if (!Flags.Contains(flag))
            {
                Flags.Add(flag);
            }

            return this;
        }

        /// <summary>
        /// Removes a flag from the Flags list.
        /// </summary>
        /// <param name="flag">The flag to remove.</param>
        /// <returns>The current instance of Options.</returns>
        public Options RemoveFlag(string flag)
        {
            Flags.Remove(flag);
            return this;
        }

        /// <summary>
        /// Provides additional JSON serialization options.
        /// </summary>
        public class JsonSerializationOptions
        {
            /// <summary>
            /// Gets or sets a value indicating whether to automatically register all entity JSON serializers during the initialization process.<br/>
            /// When set to true, all the entity-specific JSON serializers defined within the application will be registered.
            /// </summary>
            public bool UseEntityConverters { get; set; } = false;

            /// <summary>
            /// Gets or sets a value indicating whether to use the UTC DateTime converter during JSON serialization.
            /// </summary>
            public bool UseUtcDateTimeConverter { get; set; } = true;
        }
    }

}