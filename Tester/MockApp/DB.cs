using Microsoft.EntityFrameworkCore;
using ModularSystem.EntityFramework;
using MongoDB.Driver;

namespace ModularSystem.Tester;

public static class DatabaseSource
{
    public static IMongoDatabase MongoDatabase { get; }
    public static IMongoCollection<Credential> Credentials { get; }
    public static IMongoCollection<Coin> Coins { get; }
    public static IMongoCollection<Paper> Papers { get; }
    public static IMongoCollection<MongoAnt> Ants { get; }
    public static IMongoCollection<MongoTestModel> TestModel { get; }

    /// <summary>
    /// db pass: 8PLnrex8k2w8ZLPZ
    /// </summary>
    static DatabaseSource()
    {
        MongoModule.ConnectionString = "mongodb+srv://rpjcoding:8PLnrex8k2w8ZLPZ@genericdatabase.gns6dtg.mongodb.net/?retryWrites=true&w=majority";
        MongoDatabase = MongoModule.GetDatabase(MongoModule.ConnectionString, "TesterApp");

        Credentials = MongoDatabase.GetCollection<Credential>("Credentials");
        Coins = MongoDatabase.GetCollection<Coin>("Coins");
        Papers = MongoDatabase.GetCollection<Paper>("Papers");
        Ants = MongoDatabase.GetCollection<MongoAnt>("Ants");
        TestModel = MongoDatabase.GetCollection<MongoTestModel>("TestModel");
    }
}

public static class EFDatabaseSource
{
    public class AntContext : EFCoreSqliteContext<EFTestEntity>
    {
        public AntContext(FileInfo fileInfo, string tableName = "Entries") : base(fileInfo, tableName)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EFTestEntity>().OwnsOne(e => e.Email);
        }
    }

    public static DbContext TestContext()
    {
        var file = new FileInfo(Path.GetFullPath("C:\\RPJ\\TEMP\\ef_test_entity.db"));
        var context = new AntContext(file);
        return context;
    }
}