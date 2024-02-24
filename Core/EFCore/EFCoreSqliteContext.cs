using Microsoft.EntityFrameworkCore;
using ModularSystem.Core.Helpers;

namespace ModularSystem.EntityFramework;

/// <summary>
/// Represents an Entity Framework Core context specifically designed for SQLite databases.
/// </summary>
/// <typeparam name="T">The type of the entity that this context operates on. Must be a class and implement the <see cref="IEFEntity"/> interface.</typeparam>
public class EFCoreSqliteContext<T> : EFCoreContext where T : class, IEFEntity
{
    /// <summary>
    /// The default table name used in the <see cref="EFCoreSqliteContext{T}"/>.
    /// </summary>
    public const string DefaultTableName = "Entries";

    /// <summary>
    /// Gets the set of entities of type <typeparamref name="T"/> that can be queried from and written to the database.
    /// </summary>
    public DbSet<T> Entries { get; set; }

    private FileInfo FileInfo { get; }
    private string TableName { get; }

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

        FileInfo = fileInfo;
        TableName = tableName;

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

    /// <summary>
    /// Configures the DbContext options for SQLite.
    /// </summary>
    /// <param name="optionsBuilder">The options builder used to configure the DbContext.</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlite($@"Data Source={FileInfo.FullName}");
    }

    /// <summary>
    /// Configures the model for the DbContext, specifying the table name for the entity.
    /// </summary>
    /// <param name="modelBuilder">The model builder used to configure the DbContext model.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<T>().ToTable(TableName);
    }
}
