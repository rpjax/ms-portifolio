using ModularSystem.Core.Logging;

namespace ModularSystem.Core;

internal class JsonSerializerInitializer : Initializer
{
    public JsonSerializerInitializer()
    {
        Priority = (int)PriorityLevel.Normal;
    }

    public override Task InternalInitAsync(Options options)
    {
        if (options.JsonSerialization.UseEntityConverters)
        {
            //*
            // this feture is disabled.
            //*

            ConsoleLogger.Warn("Entity converters initialization has been disabled duo to development issues.");
            //RegisterEntityJsonConverters(options);
        }

        if (options.JsonSerialization.UseUtcDateTimeConverter)
        {
            RegisterUtcDateTimeJsonConverter();
        }

        return Task.CompletedTask;
    }

    void RegisterUtcDateTimeJsonConverter()
    {
        JsonSerializerSingleton.TryAddConverter(typeof(DateTime), new UtcDateTimeConverter());
    }
}
