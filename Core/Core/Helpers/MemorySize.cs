using System;

namespace ModularSystem.Core.Helpers;

/// <summary>
/// Represents a size in memory, providing methods for easy conversion and representation.
/// </summary>
public class MemorySize
{
    /// <summary>
    /// Gets or sets the size in bytes.
    /// </summary>
    public long SizeInBytes { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MemorySize"/> class.
    /// </summary>
    /// <param name="sizeInBytes">The size in bytes.</param>
    public MemorySize(long sizeInBytes)
    {
        SizeInBytes = sizeInBytes;
    }

    /// <summary>
    /// Creates a memory size representation in kilobytes.
    /// </summary>
    /// <param name="number">The number of kilobytes. Default is 1.</param>
    /// <returns>The memory size in kilobytes.</returns>
    public static MemorySize KiloByte(long number = 1)
    {
        if (number < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(number));
        }

        return new MemorySize(1024L * number);
    }

    /// <summary>
    /// Creates a memory size representation in megabytes.
    /// </summary>
    /// <param name="number">The number of megabytes. Default is 1.</param>
    /// <returns>The memory size in megabytes.</returns>
    public static MemorySize MegaByte(long number = 1)
    {
        if (number < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(number));
        }

        return new MemorySize(KiloByte().SizeInBytes * 1024L * number);
    }

    /// <summary>
    /// Creates a memory size representation in gigabytes.
    /// </summary>
    /// <param name="number">The number of gigabytes. Default is 1.</param>
    /// <returns>The memory size in gigabytes.</returns>
    public static MemorySize GigaByte(int number = 1)
    {
        if (number < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(number));
        }

        return new MemorySize(MegaByte().SizeInBytes * 1024L * number);
    }

    // INSTANCE METHODS

    /// <summary>
    /// Retrieves the memory size as an integer value. 
    /// Caution: This can cause truncation for very large sizes.
    /// </summary>
    /// <returns>The memory size as an integer.</returns>
    public int Value()
    {
        return Convert.ToInt32(SizeInBytes);
    }

    /// <summary>
    /// Retrieves the memory size as a long value.
    /// </summary>
    /// <returns>The memory size as a long.</returns>
    public long LongValue()
    {
        return SizeInBytes;
    }
}
