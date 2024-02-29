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
    /// <param name="assemblies">The assemblies to search in for initializers.</param>
    /// <param name="options">Custom options for the initialization process. If null, default options are used.</param>
    /// <remarks>
    /// This method orchestrates the initialization process by invoking the appropriate lifecycle methods on all initializers found in the specified assemblies.
    /// </remarks>
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
    /// Contains logic that runs before the main initialization process. <br/>
    /// This method runs first in the initialization pipeline and can be used for pre-initialization tasks.
    /// </summary>
    /// <param name="options">Configuration options for the initialization process.</param>
    /// <remarks>
    /// Override this method to include any setup tasks that need to be executed before the internal and main initialization logic.
    /// </remarks>
    protected virtual Task BeforeInitAsync(Options options)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Contains logic that runs internally as part of the initialization process and is invoked after <see cref="BeforeInitAsync"/>. <br/>
    /// This method is designed for the library's internal components initialization. It's executed second in the initialization pipeline.<br/>
    /// Override this method to insert custom logic that should occur after <see cref="BeforeInitAsync"/> and before <see cref="OnInitAsync"/>.
    /// </summary>
    /// <param name="options">Configuration options for the initialization process.</param>
    /// <remarks>
    /// This method provides an internal initialization hook that can be useful for setting up necessary states or configurations <br/>
    /// that are required before the main custom initialization logic (<see cref="OnInitAsync"/>) takes place.
    /// </remarks>
    protected internal virtual Task InternalInitAsync(Options options)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Contains the main logic that needs to be executed during initialization. <br/>
    /// This method runs third in the initialization pipeline, after <see cref="InternalInitAsync"/>. <br/>
    /// Derived classes must override this method to define their custom initialization behavior.
    /// </summary>
    /// <param name="options">Configuration options for the initialization process.</param>
    protected virtual Task OnInitAsync(Options options)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Contains logic that runs after the main initialization process. <br/>
    /// This method runs last in the initialization pipeline and can be used for post-initialization tasks.
    /// </summary>
    /// <param name="options">Configuration options for the initialization process.</param>
    /// <remarks>
    /// Override this method to include any finalization or cleanup tasks that need to occur after the main initialization logic.
    /// </remarks>
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
        /// Environment variables available for the initialization process.
        /// </summary>
        public Dictionary<string, string> Variables { get; set; } = new Dictionary<string, string>();

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
        /// When set to true, exceptions are logged to disk using the <see cref="ErrorLogger.EnableDiskLogging"/> mechanism.
        /// </summary>
        public bool EnableDiskExceptionLogger { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether console logging is enabled for exceptions.
        /// When set to true (default), exceptions are logged to the console using the <see cref="ErrorLogger.EnableConsoleLogging"/> mechanism.
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
        /// Attempts to get the value of an environment variable.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <returns>The value of the variable if it exists; otherwise, null.</returns>
        public string? TryGetVariable(string name)
        {
            return Variables.TryGetValue(name, out var value) ? value : null;
        }

        /// <summary>
        /// Retrieves the value of a specified environment variable.
        /// </summary>
        /// <param name="name">The name of the environment variable to retrieve.</param>
        /// <returns>The value of the environment variable if it exists.</returns>
        /// <exception cref="ErrorException">Thrown when the specified environment variable is not found.</exception>
        public string GetVariable(string name)
        {
            var value = TryGetVariable(name);

            if (value == null)
            {
                throw new ErrorException($"Environment variable '{name}' not found.");
            }

            return value;
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
