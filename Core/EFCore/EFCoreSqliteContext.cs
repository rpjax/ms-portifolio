using Microsoft.EntityFrameworkCore;
using ModularSystem.Core.Helpers;

namespace ModularSystem.EntityFramework;

/// <summary>
/// Represents a base context for Entity Framework Core operations.
/// </summary>
public class EFCoreContext : DbContext
{
 
}

/// <summary>
/// Represents an Entity Framework Core context specifically designed for SQLite databases.
/// </summary>
/// <typeparam name="T">The type of the entity that this context operates on. Must be a class and implement the <see cref="IEFModel"/> interface.</typeparam>
public class EFCoreSqliteContext<T> : DbContext where T : class, IEFModel
{
    public const string DefaultTableName = "Entries";

    /// <summary>
    /// Gets the set of entities of type <typeparamref name="T"/> that can be queried from and written to the database.
    /// </summary>
    public DbSet<T> Entries { get; set; }

    private FileInfo fileInfo { get; }
    private string tableName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EFCoreSqliteContext{T}"/> class with the specified SQLite database file and table name.
    /// </summary>
    /// <param name="fileInfo">The SQLite database file.</param>
    /// <param name="tableName">The name of the table. Defaults to "Entries" if not provided.</param>
    /// <remarks>
    /// It is recommended to use the default table name for consistency.
    /// </remarks>
    public EFCoreSqliteContext(FileInfo fileInfo, string tableName = DefaultTableName)
    {
        if (fileInfo.Extension != "db")
        {
            fileInfo = new FileInfo($"{fileInfo.FullName.Substring(0, fileInfo.FullName.Length - fileInfo.Extension.Length)}.db");
        }

        this.fileInfo = fileInfo;
        this.tableName = tableName;

        if (fileInfo.DirectoryName == null)
        {
            throw new ArgumentException(nameof(fileInfo));
        }

        FileSystemHelper.EnsureDirectoryExists(fileInfo.DirectoryName);
        Database.EnsureCreated();
    }

    /// <summary>
    /// Deletes the Write-Ahead Logging (WAL) file associated with the SQLite database.
    /// </summary>
    public void DeleteWalFile()
    {
        Database.ExecuteSqlRaw("PRAGMA wal_checkpoint(TRUNCATE);");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($@"Data Source={fileInfo.FullName}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<T>().ToTable(tableName);
    }
}
