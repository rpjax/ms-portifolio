using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ModularSystem.Mongo.Webql;

/// <summary>
/// Provides a LINQ provider implementation for MongoDB, extending the base LINQ provider functionality.
/// </summary>
public class MongoLinqProvider : ModularSystem.Webql.Synthesis.LinqProvider
{
    /// <summary>
    /// Retrieves the queryable type specific to MongoDB.
    /// </summary>
    /// <returns>The queryable type for MongoDB, which is <see cref="IMongoQueryable<>"/>.</returns>
    public override Type GetQueryableType()
    {
        return typeof(IMongoQueryable<>);
    }

    /// <summary>
    /// Retrieves the 'Where' method info from the MongoDB LINQ provider.
    /// </summary>
    /// <returns>MethodInfo for the 'Where' operation in MongoDB.</returns>
    protected override MethodInfo GetWhereMethodInfo()
    {
        return typeof(MongoQueryable).GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(m => m.Name == "Where" &&
                        m.IsGenericMethodDefinition &&
                        m.GetParameters().Length == 2 &&
                        m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IMongoQueryable<>) &&
                        m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>));
    }

    /// <summary>
    /// Retrieves the 'Select' method info from the MongoDB LINQ provider.
    /// </summary>
    /// <returns>MethodInfo for the 'Select' operation in MongoDB.</returns>
    protected override MethodInfo GetQueryableSelectMethodInfo()
    {
        return typeof(MongoQueryable).GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(m => m.Name == "Select" &&
                        m.IsGenericMethodDefinition &&
                        m.GetParameters().Length == 2 &&
                        m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IMongoQueryable<>) &&
                        m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>));
    }

    /// <summary>
    /// Retrieves the 'Take' method info from the MongoDB LINQ provider.
    /// </summary>
    /// <returns>MethodInfo for the 'Take' operation in MongoDB.</returns>
    protected override MethodInfo GetTakeMethodInfo()
    {
        return typeof(MongoQueryable).GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(m => m.Name == "Take" &&
                        m.IsGenericMethodDefinition &&
                        m.GetParameters().Length == 2 &&
                        m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IMongoQueryable<>) &&
                        m.GetParameters()[1].ParameterType == typeof(int));
    }

    /// <summary>
    /// Retrieves the 'Skip' method info from the MongoDB LINQ provider.
    /// </summary>
    /// <returns>MethodInfo for the 'Skip' operation in MongoDB.</returns>
    protected override MethodInfo GetSkipMethodInfo()
    {
        return typeof(MongoQueryable).GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(m => m.Name == "Skip" &&
                        m.IsGenericMethodDefinition &&
                        m.GetParameters().Length == 2 &&
                        m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IMongoQueryable<>) &&
                        m.GetParameters()[1].ParameterType == typeof(int));
    }

}
