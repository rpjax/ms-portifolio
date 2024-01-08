using ModularSystem.Core.Helpers;

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
    /// <summary>
    /// Gets the FileInfo object representing the file where the encryption key is stored.
    /// </summary>
    private FileInfo FileInfo { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EncryptionKeyStorage"/> class with a specified file information.
    /// </summary>
    /// <param name="fileInfo">The FileInfo object representing the file where the encryption key will be stored.</param>
    public EncryptionKeyStorage(FileInfo fileInfo)
    {
        FileInfo = fileInfo;
    }

    /// <summary>
    /// Retrieves the stored encryption key of the specified size. If no key exists, it generates and stores a new one.
    /// </summary>
    /// <param name="keySize">The size of the key that needs to be retrieved or generated.</param>
    /// <returns>A byte array representing the encryption key.</returns>
    public byte[] GetKey(int keySize)
    {
        var storage = new JsonStorage<KeyFile>(FileInfo);
        var file = storage.Read();

        if (file == null || file.Bytes.IsEmpty())
        {
            file ??= new KeyFile();
            file.Bytes = Encrypter.RandomKey(keySize);
            storage.Write(file);
        }

        if(file.Bytes.Length * 8 != (int)keySize)
        {
            file ??= new KeyFile();
            file.Bytes = Encrypter.RandomKey(keySize);
            storage.Write(file);
        }

        return file.Bytes;
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
    }
}
