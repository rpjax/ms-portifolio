using Microsoft.EntityFrameworkCore;
using ModularSystem.Core.Helpers;

namespace ModularSystem.EntityFramework;

public static class EFCoreContext
{
    public const string DEFAULT_TABLE_NAME = "Entries";
}
/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public class EFCoreContext<T> : DbContext where T : class, IEFModel
{
    public const string DEFAULT_TABLE_NAME = EFCoreContext.DEFAULT_TABLE_NAME;

    public DbSet<T> Entries { get; set; }

    private FileInfo fileInfo { get; }
    private string tableName { get; }

    /// <summary>
    /// Avoid setting the table name for consistency.
    /// </summary>
    /// <param name="fileInfo"></param>
    /// <param name="tableName"></param>
    public EFCoreContext(FileInfo fileInfo, string tableName = DEFAULT_TABLE_NAME)
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

        FileSystemHelper.InitializeDirectory(fileInfo.DirectoryName);
        Database.EnsureCreated();
    }

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
