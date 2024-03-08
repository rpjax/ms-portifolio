namespace ModularSystem.Core.Linq;

[Obsolete("WIP")]
public interface IAsyncEnumerable : System.Collections.IEnumerable
{

}

[Obsolete("WIP")]
public interface IAsyncEnumerable<T> : IEnumerable<T>, IAsyncEnumerable
{
    Task<T[]> ToArrayAsync();
}

[Obsolete("WIP")]
public static class AsyncEnumerableExtensions
{
    
}
