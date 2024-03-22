using ModularSystem.Core.Helpers;
using System.Text.Json.Serialization;

namespace ModularSystem.Core.Cryptography;

/// <summary>
/// Provides functionalities to manage and retrieve encryption keys stored in a file.
/// </summary>
/// <remarks>
/// The <see cref="EncryptionKeyStorage"/> class manages the storage and retrieval of encryption keys,
/// ensuring that if no key exists in the designated file, a new one is generated and stored.
/// </remarks>
public class EncryptionKeyStorage
{
    const string DefaultFolder = "Keys";

    public enum OnMissingFile
    {
        Throw,
        CreateNew
    }

    public enum OnKeyLengthMismatch
    {
        Throw,
        CreateNew
    }

    public OnMissingFile MissingFileStrategy { get; set; } = OnMissingFile.Throw;
    public OnKeyLengthMismatch KeyLengthMismatchStrategy { get; set; } = OnKeyLengthMismatch.Throw;

    private FileInfo FileInfo { get; }
    private int Length { get; }

    public EncryptionKeyStorage(FileInfo fileInfo, int byteLength)
    {
        FileInfo = fileInfo;
        Length = byteLength;

        if (Length <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(byteLength));
        }
    }

    public EncryptionKeyStorage(string file, int byteLength)
    {
        FileInfo = LocalStorage.GetFileInfo($"{DefaultFolder}{Path.DirectorySeparatorChar}{file}");
        Length = byteLength;

        if (Length <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(byteLength));
        }
    }

    public async Task<KeyFile> GenerateAsync()
    {
        var storage = new JsonStorage<KeyFile>(FileInfo);
        var file = new KeyFile(Encrypter.RandomKey(Length));
        await storage.WriteAsync(file);
        return file;
    }

    public async Task<byte[]> GetBytesAsync()
    {
        var storage = new JsonStorage<KeyFile>(FileInfo);
        var file = await storage.ReadAsync();

        if (file == null)
        {
            if (MissingFileStrategy == OnMissingFile.Throw)
            {
                throw MissingFileException();
            }

            file = await GenerateAsync();
        }

        if (file.Bytes.Length != Length)
        {
            if (KeyLengthMismatchStrategy == OnKeyLengthMismatch.Throw)
            {
                throw KeyLengthMismatchException(file.Bytes.Length);
            }

            file = await GenerateAsync();
        }

        return file.Bytes;
    }

    private Exception MissingFileException()
    {
        var message = $"Failed to locate the required key file at '{FileInfo.FullName}'. Ensure the file exists and the path is correct.";
        var error = new Error(message)
            .AddJsonData("key path", FileInfo.FullName)
            .AddFlags(ErrorFlags.Debug, ErrorFlags.Critical);

        return new ErrorException(error);
    }

    private Exception KeyLengthMismatchException(int length)
    {
        var message = $"Key length mismatch error. Tried to read a key with a length value different from the actual length of the key read. Expected {Length}, found {length}.";
        var error = new Error(message)
            .AddJsonData("key path", FileInfo.FullName)
            .AddFlags(ErrorFlags.Debug, ErrorFlags.Critical);

        return new ErrorException(error);
    }

}

/// <summary>
/// Represents the structure of the key file containing the encryption key in bytes.
/// </summary>
public class KeyFile
{
    /// <summary>
    /// Gets or sets the byte representation of the encryption key.
    /// </summary>
    public byte[] Bytes { get; set; } = new byte[0];

    [JsonConstructor]
    public KeyFile()
    {

    }

    public KeyFile(byte[] bytes)
    {
        Bytes = bytes;
    }
}
