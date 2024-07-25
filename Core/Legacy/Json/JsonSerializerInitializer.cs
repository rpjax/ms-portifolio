namespace ModularSystem.Core;

//internal class JsonSerializerInitializer : Initializer
//{
//    public JsonSerializerInitializer()
//    {
//        Priority = (int)PriorityLevel.Normal;
//    }

//    protected internal override Task InternalInitAsync(Options options)
//    {
//        if (options.Use)
//        {
//            RegisterUtcDateTimeJsonConverter();
//        }

//        return Task.CompletedTask;
//    }

//    void RegisterUtcDateTimeJsonConverter()
//    {
//        JsonSerializerSingleton.TryAddConverter(new UtcDateTimeConverter());
//    }
//}
