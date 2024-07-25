namespace ModularSystem.Core.Initialization;

public static partial class InitializationFlags
{
    public const string EnableInitializationLogs = "enable initialization logs";

    /// <summary>
    /// This flag configures the <see cref="EntityInitializer"/> execution.<br/>
    /// Gets or sets a value indicating whether to automatically register all entity JSON serializers during the initialization process.<br/>
    /// When set to true, all the entity-specific JSON serializers defined within the application will be registered.
    /// </summary>
    public const string RegisterAllEntityJsonSerializers = "register entity serializers into the aspnet pipeline";

    /// <summary>
    /// Gets or sets a value indicating whether to initialize the console logger.
    /// </summary>
    public const string InitConsoleLogger = "init console logger";

    public const string InitEntityKeys = "init entity keys";
}
