namespace Aidan.Core.Emulation.Components;

public class EmulatorTester
{
    public static void Execute()
    {
        var ram = new Ram();
        var emulator = new J1_CpuEmulator(ram);

        ram.WriteInstruction(
            0,
            new Instruction(
                (byte)OpCode.Load,
                (byte)RegisterType.R0,
                (byte)AddressingMode.Immediate,
                (byte)WordSize.DWord,
                1, 0, 0, 0
            )
        );

        ram.WriteInstruction(
            8,
            new Instruction(
                opCode: (byte)OpCode.Load,
                (byte)RegisterType.R1,
                (byte)AddressingMode.Immediate,
                (byte)WordSize.DWord,
                55, 0, 0, 0
            )
        );

        emulator.Run();
    }
}

public class Ram : IIoInterface
{
    public const int DefaultSize = 1024 * 16;
    public const int MaxSize = 1024 * 1024 * 10;

    private byte[] Data { get; }

    public Ram(int size = DefaultSize)
    {
        if (size < 0 || size > MaxSize)
        {
            throw new ArgumentOutOfRangeException(nameof(size));
        }

        Data = new byte[size];
    }

    public byte this[uint address]
    {
        get => Read(address);
        set => Write(address, value);
    }

    public byte Read8(uint address)
    {
        return Read(address);
    }

    public void Write8(uint address, byte value)
    {
        Write(address, value);
    }

    public ushort Read16(uint address)
    {
        return (ushort)(Read(address) | (Read(address + 1) << 8));
    }

    public void Write16(uint address, ushort value)
    {
        Write(address, (byte)(value & 0xFF));
        Write(address + 1, (byte)(value >> 8));
    }

    public uint Read32(uint address)
    {
        return (uint)(Read(address) | (Read(address + 1) << 8) | (Read(address + 2) << 16) | (Read(address + 3) << 24));
    }

    public void Write32(uint address, uint value)
    {
        Write(address, (byte)(value & 0xFF));
        Write(address + 1, (byte)((value >> 8) & 0xFF));
        Write(address + 2, (byte)((value >> 16) & 0xFF));
        Write(address + 3, (byte)((value >> 24) & 0xFF));
    }

    public void WriteInstruction(uint address, Instruction instruction)
    {
        Write(address, instruction.OpCode);

        for (uint i = 0; i < instruction.Operands.Length; i++)
        {
            Write(address + 1 + i, instruction.Operands[i]);
        }
    }

    private void Write(uint address, byte value)
    {
        if (address < 0 || address >= Data.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(address));
        }

        Data[address] = value;
    }

    private byte Read(uint address)
    {
        if (address < 0 || address >= Data.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(address));
        }

        return Data[address];
    }
}

public interface IIoInterface
{
    byte Read8(uint address);
    void Write8(uint address, byte value);

    ushort Read16(uint address);
    void Write16(uint address, ushort value);

    uint Read32(uint address);
    void Write32(uint address, uint value);
}

public enum OpCode : byte
{
    NoOp,

    /*
        Memory operations.
    */
    Load,
    Store,
    Move,

    /*
        Control flow operations.
    */
    Jump,
    Call,
    Return,
    InterruptReturn,

    /*
        Arithmetic operations.
    */
    Add,
    Subtract,
    Multiply,
    Divide
}

public enum InterruptType : byte
{
    NoOp,
    Halt,
    Callback
}

public enum RegisterType : byte
{
    ProgramCounter,
    StackPointer,
    R0,
    R1,
    R2
}

public enum AddressingMode : byte
{
    Immediate,
    Register,
    Indirect
}

public enum WordSize : byte
{
    /// <summary>
    /// 8 bits
    /// </summary>
    Byte,

    /// <summary>
    /// 16 bits
    /// </summary>
    Word,

    /// <summary>
    /// 32 bits
    /// </summary>
    DWord,

    // /// <summary>
    // /// 64 bits
    // /// </summary>
    // QWord,
}

[Flags]
public enum CpuFlag : uint
{
    InterruptEnable = 1 << 0,
    InterruptRequested = 1 << 1,
}

public class Register8
{
    public byte Value { get; set; }

    public static implicit operator byte(Register8 register)
    {
        return register.Value;
    }

    public void Increment()
    {
        Value++;
    }

    public void Decrement()
    {
        Value--;
    }

    public Register8 Copy()
    {
        return new Register8 { Value = Value };
    }

}

public class Register32
{
    public uint Value { get; set; }

    public static implicit operator uint(Register32 register)
    {
        return register.Value;
    }

    public void Increment(uint value = 1)
    {
        Value += value;
    }

    public void Decrement(uint value = 1)
    {
        Value -= value;
    }

    public Register32 Copy()
    {
        return new Register32 { Value = Value };
    }
}

public class FlagsRegister32
{
    public uint Value { get; set; }

    public static implicit operator uint(FlagsRegister32 register)
    {
        return register.Value;
    }

    public FlagsRegister32 Copy()
    {
        return new FlagsRegister32 { Value = Value };
    }

    public void SetFlag(CpuFlag flag)
    {
        Value |= (uint)flag;
    }

    public void ClearFlag(CpuFlag flag)
    {
        Value &= ~(uint)flag;
    }

    public bool IsFlagSet(CpuFlag flag)
    {
        return (Value & (uint)flag) != 0;
    }

    public void ClearAllFlags()
    {
        Value = 0;
    }

}

public class Instruction
{
    public byte OpCode { get; }
    public byte[] Operands { get; }

    public Instruction(byte opCode, params byte[] operands)
    {
        OpCode = opCode;
        Operands = operands;
    }

    public override string ToString()
    {
        return $"OpCode: {(OpCode)OpCode}, Operands: {string.Join(", ", Operands)}";
    }
}

public class InstructionBuilder
{
    private byte OpCode { get; set; }
    private List<byte> Operands { get; } = new();

    public InstructionBuilder SetOpCode(byte opCode)
    {
        OpCode = opCode;
        return this;
    }

    public InstructionBuilder SetRegister(RegisterType registerType)
    {
        Operands.Add((byte)registerType);
        return this;
    }

    public InstructionBuilder SetAddressingMode(AddressingMode addressingMode)
    {
        Operands.Add((byte)addressingMode);
        return this;
    }

    public InstructionBuilder SetWordSize(WordSize wordSize)
    {
        Operands.Add((byte)wordSize);
        return this;
    }

    public InstructionBuilder SetByte(byte value)
    {
        Operands.Add(value);
        return this;
    }

    public InstructionBuilder SetWord(ushort value)
    {
        Operands.Add((byte)(value & 0xFF));
        Operands.Add((byte)(value >> 8));
        return this;
    }

    public InstructionBuilder SetDWord(uint value)
    {
        Operands.Add((byte)(value & 0xFF));
        Operands.Add((byte)((value >> 8) & 0xFF));
        Operands.Add((byte)((value >> 16) & 0xFF));
        Operands.Add((byte)((value >> 24) & 0xFF));
        return this;
    }

    public Instruction Build()
    {
        return new Instruction(OpCode, Operands.ToArray());
    }

}

// little endian (LSB first)

public class Interrupt
{
    public InterruptType Type { get; set; }
    public uint Data { get; set; }

    public Interrupt(InterruptType type, uint data)
    {
        Type = type;
        Data = data;
    }

    public static Interrupt NoOp()
    {
        return new Interrupt(InterruptType.NoOp, 0);
    }

    public override string ToString()
    {
        return $"{Type}";
    }
}

public class Context
{
    public Register32 ProgramCounter { get; }
    public FlagsRegister32 FlagsRegister { get; }

    public Context(
        Register32 programCounter,
        FlagsRegister32 flagsRegister
    )
    {
        ProgramCounter = programCounter;
        FlagsRegister = flagsRegister;
    }
}

public class J1_CpuEmulator
{
    private const uint InstructionSize = 1;

    private IIoInterface IoInterface { get; }

    private Register32 ProgramCounter { get; } = new();
    private Register8 InstructionRegister { get; } = new();
    private FlagsRegister32 FlagsRegister { get; } = new();
    private Register32 StackPointer { get; } = new();
    private Register32 InterruptHandler { get; } = new();
    private Register32 R0 { get; } = new();
    private Register32 R1 { get; } = new();
    private Register32 R2 { get; } = new();

    private ImmediateReader ImmediateValue { get; }
    private Interrupt InterruptRequest { get; } = Interrupt.NoOp();
    private bool IsRunning { get; set; }

    public J1_CpuEmulator(IIoInterface ioInterface)
    {
        IoInterface = ioInterface;
        ImmediateValue = new ImmediateReader(this);
    }

    class ImmediateReader
    {
        private J1_CpuEmulator Cpu { get; }
        public uint BytesRead { get; private set; }

        public ImmediateReader(J1_CpuEmulator cpu)
        {
            Cpu = cpu;
        }

        public void Reset()
        {
            BytesRead = 0;
        }

        public byte Read8()
        {
            return Cpu.ReadImmediate8(IncrementOffset(1));
        }

        public ushort Read16()
        {
            return Cpu.ReadImmediate16(IncrementOffset(2));
        }

        public uint Read32()
        {
            return Cpu.ReadImmediate32(IncrementOffset(4));
        }

        public RegisterType ReadRegisterType()
        {
            return (RegisterType)Read8();
        }

        public AddressingMode ReadAddressingMode()
        {
            return (AddressingMode)Read8();
        }

        public WordSize ReadWordSize()
        {
            return (WordSize)Read8();
        }

        public uint IncrementOffset(uint value)
        {
            var offset = BytesRead;
            BytesRead += value;
            return offset;
        }

    }

    public void Run()
    {
        IsRunning = true;

        while (IsRunning)
        {
            Cicle();
        }
    }

    public void Stop()
    {
        IsRunning = false;
    }

    public void Cicle()
    {
        if (FlagsRegister.IsFlagSet(CpuFlag.InterruptRequested))
        {
            HandleInterupt();
        }

        Fetch();
        Execute();
        ResetCicle();
    }

    private void Fetch()
    {
        InstructionRegister.Value = IoInterface.Read8(ProgramCounter.Value);
    }

    private void Execute()
    {
        switch (DecodeOpCode())
        {
            case OpCode.NoOp:
                return;

            case OpCode.Load:
                Load();
                return;

            case OpCode.Store:
                Store();
                return;

            case OpCode.Move:
                Move();
                return;

            case OpCode.Jump:
                Jump();
                return;

            case OpCode.InterruptReturn:
                InterruptReturn();
                return;

            default:
                throw new InvalidOperationException();
        }
    }

    private void HandleInterupt()
    {
        FlagsRegister.ClearFlag(CpuFlag.InterruptRequested);

        switch (InterruptRequest.Type)
        {
            case InterruptType.NoOp:
                break;

            case InterruptType.Halt:
                IsRunning = false;
                break;

            case InterruptType.Callback:
                PushContextToStack();
                ProgramCounter.Value = InterruptHandler.Value;
                break;

            default:
                throw new InvalidOperationException();
        }

        InterruptRequest.Type = InterruptType.NoOp;
    }

    private void ResetCicle()
    {
        ProgramCounter.Increment(InstructionSize + ImmediateValue.BytesRead);
        ImmediateValue.Reset();
    }

    /*
        Internal helpers.
    */
    private OpCode DecodeOpCode()
    {
        return (OpCode)InstructionRegister.Value;
    }

    private byte ReadImmediate8(uint offsetInBytes = 0)
    {
        return IoInterface.Read8(ProgramCounter.Value + InstructionSize + offsetInBytes);
    }

    private ushort ReadImmediate16(uint offsetInBytes = 0)
    {
        return IoInterface.Read16(ProgramCounter.Value + InstructionSize + offsetInBytes);
    }

    private uint ReadImmediate32(uint offsetInBytes = 0)
    {
        return IoInterface.Read32(ProgramCounter.Value + InstructionSize + offsetInBytes);
    }

    private Register32 GetRegister(RegisterType registerType)
    {
        switch (registerType)
        {
            case RegisterType.ProgramCounter:
                return ProgramCounter;

            case RegisterType.StackPointer:
                return StackPointer;

            case RegisterType.R0:
                return R0;

            case RegisterType.R1:
                return R1;

            case RegisterType.R2:
                return R2;

            default:
                throw new InvalidOperationException();
        }
    }

    private Context GetContext()
    {
        return new Context(
            programCounter: ProgramCounter.Copy(),
            flagsRegister: FlagsRegister.Copy()
        );
    }

    private void Push8ToStack(byte value)
    {
        StackPointer.Decrement();
        IoInterface.Write8(StackPointer, value);
    }

    private void Push16ToStack(ushort value)
    {
        StackPointer.Decrement();
        IoInterface.Write8(StackPointer, (byte)(value & 0xFF));
        StackPointer.Decrement();
        IoInterface.Write8(StackPointer, (byte)(value >> 8));
    }

    private void Push32ToStack(uint value)
    {
        StackPointer.Decrement();
        IoInterface.Write8(StackPointer, (byte)(value & 0xFF));
        StackPointer.Decrement();
        IoInterface.Write8(StackPointer, (byte)((value >> 8) & 0xFF));
        StackPointer.Decrement();
        IoInterface.Write8(StackPointer, (byte)((value >> 16) & 0xFF));
        StackPointer.Decrement();
        IoInterface.Write8(StackPointer, (byte)((value >> 24) & 0xFF));
    }

    private byte Pop8FromStack()
    {
        var value = IoInterface.Read8(StackPointer);
        StackPointer.Increment();
        return value;
    }

    private ushort Pop16FromStack()
    {
        var value = (ushort)(IoInterface.Read8(StackPointer) << 8);
        StackPointer.Increment();
        value |= IoInterface.Read8(StackPointer);
        StackPointer.Increment();
        return value;
    }

    private uint Pop32FromStack()
    {
        var value = (uint)(IoInterface.Read8(StackPointer) << 24);
        StackPointer.Increment();
        value |= (uint)(IoInterface.Read8(StackPointer) << 16);
        StackPointer.Increment();
        value |= (uint)(IoInterface.Read8(StackPointer) << 8);
        StackPointer.Increment();
        value |= IoInterface.Read8(StackPointer);
        StackPointer.Increment();
        return value;
    }

    private void PushContextToStack()
    {
        Push32ToStack(ProgramCounter.Value);
        Push32ToStack(FlagsRegister.Value);
    }

    private void RestoreContextFromStack()
    {
        FlagsRegister.Value = Pop32FromStack();
        ProgramCounter.Value = Pop32FromStack();
    }

    /*
        Instructions execution.
    */
    private void Load()
    {
        var destination = ImmediateValue.ReadRegisterType();
        var addressingMode = ImmediateValue.ReadAddressingMode();
        var wordSize = ImmediateValue.ReadWordSize();
        var @base = ImmediateValue.Read32();



        GetRegister(destination).Value = @base;
    }

    private void Store()
    {
        var source = ImmediateValue.ReadRegisterType();
        var addressingMode = ImmediateValue.ReadAddressingMode();
        var wordSize = ImmediateValue.ReadWordSize();
        var destination = ImmediateValue.Read32();

        var value = GetRegister(source).Value;

        IoInterface.Write32(destination, value);
    }

    private void Move()
    {
        var sourceRegisterType = ImmediateValue.ReadRegisterType();
        var destinationRegisterType = ImmediateValue.ReadRegisterType();

        var sourceRegister = GetRegister(sourceRegisterType);
        var destinationRegister = GetRegister(destinationRegisterType);

        destinationRegister.Value = sourceRegister.Value;
    }

    private void Jump()
    {
        ProgramCounter.Value = ImmediateValue.Read32();
    }

    private void InterruptReturn()
    {
        RestoreContextFromStack();
    }


}
