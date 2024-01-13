using ModularSystem.Core;
using System.Linq.Expressions;

namespace ModularSystem.Web.Linq;

/// <summary>
/// Provides LINQ extension methods for <see cref="ServiceQueryable"/> to enable fluent, type-safe querying on remote data sources.
/// </summary>
/// <remarks>
/// These extension methods extend <see cref="ServiceQueryable"/> with common LINQ query operators, <br/> 
/// facilitating the construction of complex queries in a type-safe manner. <br/>
/// They ensure that queries remain fluent and readable, while being executed remotely against a data service.
/// </remarks>
public static class ServiceQueryableLinq
{
    /// <summary>
    /// Filters a sequence of values based on a predicate and returns a remote queryable sequence.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the source sequence.</typeparam>
    /// <param name="source">An <see cref="ServiceQueryable{TSource}"/> to filter.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <returns>An <see cref="ServiceQueryable{TSource}"/> that contains elements from the input sequence that satisfy the condition.</returns>
    public static ServiceQueryable<TSource> Where<TSource>(this ServiceQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
    {
        return (ServiceQueryable<TSource>)source.AsQueryable().Where(predicate);
    }

    /// <summary>
    /// Projects each element of a sequence into a new form and returns a remote queryable sequence.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the source sequence.</typeparam>
    /// <typeparam name="TResult">The type of the value returned by the selector function.</typeparam>
    /// <param name="source">An <see cref="ServiceQueryable{TSource}"/> to project.</param>
    /// <param name="selector">A transform function to apply to each element.</param>
    /// <returns>An <see cref="ServiceQueryable{TResult}"/> whose elements are the result of invoking the transform function on each element of the source.</returns>
    public static ServiceQueryable<TResult> Select<TSource, TResult>(this ServiceQueryable<TSource> source, Expression<Func<TSource, TResult>> selector)
    {
        return (ServiceQueryable<TResult>)source.AsQueryable().Select(selector);
    }

    /// <summary>
    /// Ignora um número especificado de elementos em uma sequência e retorna o restante dos elementos.
    /// </summary>
    /// <typeparam name="TSource">O tipo dos elementos da sequência de origem.</typeparam>
    /// <param name="source">A sequência de origem.</param>
    /// <param name="count">O número de elementos a serem ignorados.</param>
    /// <returns>Um ServiceQueryable que contém os elementos que ocorrem após os elementos ignorados na sequência de origem.</returns>
    public static ServiceQueryable<TSource> Skip<TSource>(this ServiceQueryable<TSource> source, int count)
    {
        return (ServiceQueryable<TSource>)source.AsQueryable().Skip(count);
    }

    /// <summary>
    /// Retorna um número especificado de elementos contíguos do início de uma sequência.
    /// </summary>
    /// <typeparam name="TSource">O tipo dos elementos da sequência de origem.</typeparam>
    /// <param name="source">A sequência de origem.</param>
    /// <param name="count">O número de elementos a serem retornados.</param>
    /// <returns>Um ServiceQueryable que contém o número especificado de elementos do início da sequência de origem.</returns>
    public static ServiceQueryable<TSource> Take<TSource>(this ServiceQueryable<TSource> source, int count)
    {
        return (ServiceQueryable<TSource>)source.AsQueryable().Take(count);
    }

    /// <summary>
    /// Ordena os elementos de uma sequência em ordem ascendente com base em uma chave.
    /// </summary>
    /// <typeparam name="TSource">O tipo dos elementos da sequência de origem.</typeparam>
    /// <typeparam name="TKey">O tipo da chave pela qual ordenar.</typeparam>
    /// <param name="source">A sequência de origem a ser ordenada.</param>
    /// <param name="keySelector">Uma função para extrair a chave para cada elemento.</param>
    /// <returns>Um ServiceQueryable cujos elementos são ordenados em ordem ascendente de acordo com uma chave.</returns>
    public static ServiceQueryable<TSource> OrderBy<TSource, TKey>(this ServiceQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
    {
        return (ServiceQueryable<TSource>)source.AsQueryable().OrderBy(keySelector);
    }

    /// <summary>
    /// Ordena os elementos de uma sequência em ordem descendente com base em uma chave.
    /// </summary>
    /// <typeparam name="TSource">O tipo dos elementos da sequência de origem.</typeparam>
    /// <typeparam name="TKey">O tipo da chave pela qual ordenar.</typeparam>
    /// <param name="source">A sequência de origem a ser ordenada.</param>
    /// <param name="keySelector">Uma função para extrair a chave para cada elemento.</param>
    /// <returns>Um ServiceQueryable cujos elementos são ordenados em ordem descendente de acordo com uma chave.</returns>
    public static ServiceQueryable<TSource> OrderByDescending<TSource, TKey>(this ServiceQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
    {
        return (ServiceQueryable<TSource>)source.AsQueryable().OrderByDescending(keySelector);
    }

    //*
    // Materialization methods.
    //*

    /// <summary>
    /// Asynchronously creates an array from a <see cref="ServiceQueryable{TSource}"/>.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the source sequence.</typeparam>
    /// <param name="source">An <see cref="ServiceQueryable{TSource}"/> to create an array from.</param>
    /// <returns>A task that represents the asynchronous operation. <br/>
    /// The task result contains an array that contains elements from the input sequence.
    /// </returns>
    /// <remarks>
    /// This method asynchronously sends the query expression to the remote data service, executes it, <br/>
    /// and returns the results as an array, offering a convenient way to retrieve the query results.
    /// </remarks>
    public static async Task<TSource[]> ToArrayAsync<TSource>(this ServiceQueryable<TSource> source)
    {
        return (await ((ServiceQueryProvider<TSource>)source.Provider).ExecuteAsync(source.Expression)).ToArray();
    }

    //TODO...
    //public static async Task<int> CountAsync<TSource>(this ServiceQueryable<TSource> source)
    //{
    //    return (await source.Select(x => new {}).ToArrayAsync()).Length;
    //}

}
