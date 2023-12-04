using DnsClient;
using ModularSystem.Core;
using ModularSystem.Core.Cli;
using ModularSystem.EntityFramework;
using ModularSystem.Mailing;
using ModularSystem.Mongo;
using ModularSystem.Web;
using ModularSystem.Web.Expressions;
using MongoDB.Bson;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace ModularSystem.Tester;

public partial class Sandbox : CliCommand
{
    public Sandbox(CLI cli, PromptContext context) : base(cli, context)
    {

    }

    class Env
    {
        public string? Environment { get; set; }
        public string[]? Uris { get; set; }
    }

    protected override async void Execute()
    {
        using var service = new EFTestService();

        //for (int i = 0; i < 100; i++)
        //{
        //    service.CreateAsync(new EFTestEntity()
        //    {
        //        Nickname = "jacques",
        //        FirstName = "Rodrigo",
        //        Email = new("jacques@more.tt")
        //    }).Wait();  
        //}

        //await Console.Out.WriteLineAsync("success");
        //return;

        var query = new QueryWriter<EFTestEntity>()
                .SetLimit(50)
                .SetFilter(x => x.Nickname.ToLower().Contains("amanda"))
                .CreateSerializable();

        var rebQuery = query.ToQuery<EFTestEntity>();

        var update = new UpdateWriter<EFTestEntity>()
                .SetFilter(x => x.Id <= 90)
                .SetModification(x => x.Email.Username, "yummandy")
                //.SetModification(x => x.Email.Domain, "gmail")
                //.SetModification(x => x.Email.Extension, "com")
                .CreateSerializable();

        var rebUpdate = update.ToUpdate<EFTestEntity>();

        await service.UpdateAsync(rebUpdate);
        var queryResult = await service.QueryAsync(rebQuery);

        await Console.Out.WriteLineAsync();
    }

    public override string Instruction()
    {
        return "sandbox";
    }

    public class DateTimeTest
    {
        public DateTime Time { get; set; }
    }
}

class MyMiddleware : EntityMiddleware<MongoTestModel>
{
    public override Task<IQuery<MongoTestModel>> BeforeQueryAsync(IQuery<MongoTestModel> query)
    {
        Console.WriteLine("before query!");
        return base.BeforeQueryAsync(query);
    }

    public override Task<IUpdate<MongoTestModel>> BeforeUpdateAsync(IUpdate<MongoTestModel> update)
    {
        Console.WriteLine("before update!");
        return base.BeforeUpdateAsync(update);
    }
}

public class MongoTestModel : MongoModel
{
    public string FirstName { get; set; } = string.Empty;
    public string[] Surnames { get; set; } = Array.Empty<string>();
    public string? Nickname { get; set; }
    public Email Email { get; set; } = Email.Empty();
}

public class MongoTestEntity : MongoEntityService<MongoTestModel>
{
    public override IDataAccessObject<MongoTestModel> DataAccessObject { get; }

    public MongoTestEntity()
    {
        DataAccessObject = new MongoDataAccessObject<MongoTestModel>(MongoDb.TestModel);
        Validator = new EmptyValidator<MongoTestModel>();
        UpdateValidator = new EmptyValidator<MongoTestModel>();
    }
}

public interface IRegister<T>
{
    T Value { get; set; }
    void Increment();
}

public struct Register16Bits : IRegister<ushort>
{
    public ushort Value { get; set; }

    public void Increment()
    {
        Value++;
    }
}

public struct Word16Bits
{
    public ushort Value { get; }

    public Word16Bits(ushort value)
    {
        Value = value;
    }
}

public struct Word8Bits
{
    public byte Value { get; }

    public Word8Bits(byte value)
    {
        Value = value;
    }
}

[Flags]
public enum Flags8Bits : byte
{
    Carry = 1,           
    Zero = 2,            
    InterruptDisable = 4,   
    DecimalMode = 8,     
    Overflow = 16,       
    Negative = 32        
}

public class SizedMemory
{
    public long Size { get; }
    private byte[] Bytes { get; }

    public SizedMemory(long size)
    {
        Size = size;
        Bytes = new byte[size];
    }
}

public class EmulatedClock
{

}

public class ClockSignalEvent
{

}

public struct IoEvent
{
    public Flags8Bits Flags { get; }
    public Word16Bits Address { get; }
    public Word8Bits Data { get; }

    public IoEvent(Flags8Bits flags, Word16Bits address)
    {
        Flags = flags;
        Address = address;
        Data = new Word8Bits();
    }

    public IoEvent(Flags8Bits flags, Word16Bits address, Word8Bits data)
    {
        Flags = flags;
        Address = address;
        Data = data;
    }
}

public class IoController
{
    private int MaxEvents { get; set; } = 5;
    private ConcurrentQueue<IoEvent> EventsQueue { get; } = new();

    public Task DispatchAsync(IoEvent ioEvent)
    {
        return Task.CompletedTask;
    }
}

public enum CpuState
{

}

public class CpuCoreEmulator
{
    private Register16Bits ProgramCounter { get; } = new();
    private Register16Bits Accumulator { get; } = new();
    private Register16Bits XIndex { get; } = new();
    private Register16Bits YIndex { get; } = new();
    private Register16Bits ZIndex { get; } = new();
    private IoController IoController { get; } = new();

    private ConcurrentDictionary<string, Word16Bits> InterruptHandlers { get; } = new();

    private void OnClockSignal(ClockSignalEvent clockSignal)
    {
        ProgramCounter.Increment();
        StartIo(Flags8Bits.InterruptDisable | Flags8Bits.Negative, new(0x00));
    }

    private void StartIo(Flags8Bits flags, Word16Bits address, Word8Bits? data = null)
    {
        IoController.DispatchAsync(new IoEvent(flags, address, data ?? new())).Wait();
    }

    private void OnIo(IoEvent ioEvent)
    {
        
    }

}