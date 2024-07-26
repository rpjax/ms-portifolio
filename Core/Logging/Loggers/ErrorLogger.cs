using Microsoft.EntityFrameworkCore;
using ModularSystem.Core.Linq;
using ModularSystem.Core.Patterns;
using ModularSystem.Core.Threading;
using ModularSystem.EntityFramework;
using ModularSystem.EntityFramework.Repositories;

namespace ModularSystem.Core.Logging;

public class ErrorLoggerSettings
{
    public FileInfo? FileInfo { get; set; } 
    public bool EnableConsoleLogging { get; set; } 
    public bool EnableDiskLogging { get; set; }

    public ErrorLoggerSettings(
        FileInfo? fileInfo = null, 
        bool enableConsoleLogging = false, 
        bool enableDiskLogging = true)
    {
        FileInfo = fileInfo;
        EnableConsoleLogging = enableConsoleLogging;
        EnableDiskLogging = enableDiskLogging;
    }
}

/// <summary>
/// Provides a centralized mechanism for logging errors.<br/>
/// This logger has the capability to log errors both to the console and to a disk-based log file.
/// </summary>
/// <remarks>
/// By default, this logger saves errors to a file named 'e_debug.db' located in the '/LOGS' directory.<br/>
/// Disk logging can be toggled on or off, and it uses a SQLite-based logger for persistence.
/// </remarks>
public class ErrorLogger : IErrorLogger
{
    /// <summary>
    /// The default filename used to save the log entries to disk.
    /// </summary>
    public const string DefaultFileName = "e_debug.db";

    /// <summary>
    /// Gets the data access object used for interacting with the database.
    /// </summary>
    private IRepository<ErrorEntry> Repository { get; }

    /*
     * Settings
     */

    private bool EnableConsoleLogging { get; }
    private bool EnableDiskLogging { get; }

    internal class EntriesDbContext : EFCoreSqliteContext
    {
        public DbSet<ErrorEntry> Entries { get; set; }

        public EntriesDbContext(FileInfo fileInfo) : base(fileInfo)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ErrorEntry>()
                .HasMany(e => e.Flags)
                .WithOne()
                .IsRequired();

            modelBuilder.Entity<ErrorEntry>()
                .HasMany(e => e.Details)
                .WithOne()
                .IsRequired();

            modelBuilder.Entity<ErrorEntry>()
                .HasMany(e => e.Data)
                .WithOne()
                .IsRequired();  
        }

    }

    public ErrorLogger(ErrorLoggerSettings? settings = null)
    {
        
        settings ??= new ErrorLoggerSettings();
        Repository = CreateRepository(settings.FileInfo ?? LogsDirectory.GetFileInfo(DefaultFileName));
        EnableConsoleLogging = settings.EnableConsoleLogging;
        EnableDiskLogging = settings.EnableDiskLogging;
    }

    public void Log(Error error)
    {
        if (EnableConsoleLogging)
        {
            Console.WriteLine(error.ToString());
        }

        if (EnableDiskLogging)
        {
            JobQueue.Enqueue(CreateJob(error));
        }
    }

    public IAsyncQueryable<ErrorEntry> AsQueryable()
    {
        return Repository.AsQueryable();
    }

    /*
     * Private helpers
     */

    private Job CreateJob(Error error)
    {
        return new LambdaJob(async (cancellationToken) =>
        {
            await Repository.CreateAsync(new ErrorEntry(error));
        });
    }

    private static IRepository<ErrorEntry> CreateRepository(FileInfo fileInfo)
    {
        return new EFCoreRepository<ErrorEntry>(new EntriesDbContext(fileInfo));
    }

}

