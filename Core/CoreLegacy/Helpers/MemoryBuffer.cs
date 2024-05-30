using System.Text;

namespace ModularSystem.Core.Helpers;

/// <summary>
/// A thread safe self wrapping buffer implementation. Performance requires testing.
/// </summary>
public class MemoryBuffer
{
    public int Length => length;
    public int StoredBytes => storedBytes;
    public int AvailableSpace => CountWriteAvailableBytes();
    public bool IsFull => storedBytes > 0 && head == tail || storedBytes == length;
    public bool IsEmpty => storedBytes == 0;
    public bool IsNotEmpty => !IsEmpty;
    public bool IsWrappedAround => head > tail || IsFull && head == tail;

    public Encoding Encoding { get; set; } = Encoding.UTF8;

    readonly int length;
    int storedBytes = 0;
    int head = 0;
    int tail = 0;

    byte[] buffer;

    object mutex = new object();

    public MemoryBuffer(int size_in_bytes)
    {
        if (size_in_bytes < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(size_in_bytes));
        }

        length = size_in_bytes;
        buffer = new byte[size_in_bytes];
    }

    public int CountWriteAvailableBytes()
    {
        return length - storedBytes;
    }

    public int CountSequentialWriteAvailableBytes()
    {
        if (IsFull)
        {
            return 0;
        }

        if (IsWrappedAround)
        {
            return head - tail;
        }
        else
        {
            return length - tail;
        }
    }

    public int CountSequentialReadAvailableBytes()
    {
        if (IsEmpty)
        {
            return length;
        }

        if (IsWrappedAround)
        {
            return length - head;
        }
        else
        {
            return tail - head;
        }
    }

    public double UsagePercentage()
    {
        return Convert.ToDouble(storedBytes) / Convert.ToDouble(length);
    }

    public bool CanWrite(int length)
    {
        if (length < 0)
        {
            throw new ArgumentOutOfRangeException();
        }

        return CountWriteAvailableBytes() >= length;
    }

    public void Write(byte[] bytes)
    {
        if (bytes.IsEmpty())
        {
            throw new ArgumentOutOfRangeException(nameof(bytes));
        }

        lock (mutex)
        {
            int availableBytes = CountWriteAvailableBytes();
            int sequentialAvailableBytes = CountSequentialWriteAvailableBytes();

            if (bytes.Length > availableBytes)
            {
                throw new ArgumentOutOfRangeException(nameof(bytes));
            }

            if (bytes.Length > sequentialAvailableBytes)
            {
                NonSequentialWrite(bytes);
            }
            else
            {
                SequentialWrite(bytes);
            }

            storedBytes += bytes.Length;
        }
    }

    public void Write(string text)
    {
        Write(Encoding.GetBytes(text));
    }

    public byte[] Read(int length_in_bytes, bool consume = true)
    {
        if (length_in_bytes < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length_in_bytes));
        }
        if (length_in_bytes == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length_in_bytes));
        }
        if (IsEmpty)
        {
            throw new InvalidOperationException("The buffer is empty.");
        }

        lock (mutex)
        {
            int sequentialAvailableBytes = CountSequentialReadAvailableBytes();

            if (length_in_bytes > StoredBytes)
            {
                throw new ArgumentOutOfRangeException(nameof(length_in_bytes));
            }

            byte[] readBuffer;

            if (length_in_bytes > sequentialAvailableBytes)
            {
                readBuffer = NonSequentialRead(length_in_bytes, consume);
            }
            else
            {
                readBuffer = SequentialRead(length_in_bytes, consume);
            }

            if (consume)
            {
                storedBytes -= readBuffer.Length;
            }

            return readBuffer;
        }
    }

    public byte ReadByte(bool consume = true)
    {
        return Read(1, consume).First();
    }

    public string ReadText(int length_in_bytes, bool consume = true)
    {
        return Encoding.GetString(Read(length_in_bytes, consume));
    }

    public string[] ReadLines(bool consume = true)
    {
        if (IsEmpty)
        {
            return new string[0];
        }

        return ReadText(StoredBytes, consume).Split(Environment.NewLine);
    }

    public byte[] PeekRead(int length_in_bytes, int offset)
    {
        if (offset + length_in_bytes > storedBytes)
        {
            throw new ArgumentOutOfRangeException($"{nameof(offset)} {nameof(length_in_bytes)}");
        }

        try
        {
            head += offset;
            return Read(length_in_bytes, false);
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            head -= offset;
        }
    }

    public void Reset()
    {
        head = 0;
        tail = 0;
        storedBytes = 0;
    }

    public override string ToString()
    {
        return Encoding.GetString(buffer);
    }

    void NonSequentialWrite(byte[] bytes)
    {
        int sequentialAvailableBytes = CountSequentialWriteAvailableBytes();
        int firstWriteLength = sequentialAvailableBytes;
        int secondWriteLength = bytes.Length - sequentialAvailableBytes;

        Array.Copy(bytes, 0, buffer, tail, firstWriteLength);
        Array.Copy(bytes, firstWriteLength, buffer, 0, secondWriteLength);

        tail = secondWriteLength;

        if (tail > length)
        {
            throw new InvalidOperationException();
        }
        if (tail == length)
        {
            tail = 0;
        }
    }

    void SequentialWrite(byte[] bytes)
    {
        if (tail + bytes.Length > length)
        {
            throw new InvalidOperationException();
        }

        Array.Copy(bytes, 0, buffer, tail, bytes.Length);
        tail = tail + bytes.Length;

        if (tail > length)
        {
            throw new InvalidOperationException();
        }
        if (tail == length)
        {
            tail = 0;
        }
    }

    byte[] NonSequentialRead(int length, bool consume)
    {
        if (!IsWrappedAround)
        {
            throw new InvalidOperationException();
        }

        int firstReadLength = this.length - head;
        int secondReadLength = length - firstReadLength;
        byte[] readBuffer = new byte[length];

        Array.Copy(buffer, head, readBuffer, 0, firstReadLength);
        Array.Copy(buffer, 0, readBuffer, firstReadLength, secondReadLength);

        if (consume)
        {
            head = secondReadLength;
        }

        if (head > this.length)
        {
            throw new InvalidOperationException();
        }
        if (head == this.length)
        {
            head = 0;
        }

        return readBuffer;
    }

    byte[] SequentialRead(int length, bool consume)
    {
        byte[] readBuffer = new byte[length];

        Array.Copy(buffer, head, readBuffer, 0, length);

        if (consume)
        {
            head += length;
        }

        if (head > this.length)
        {
            throw new InvalidOperationException();
        }
        if (head == this.length)
        {
            head = 0;
        }

        return readBuffer;
    }
}