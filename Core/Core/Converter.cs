namespace ModularSystem.Core;

public class Converter<T1, T2> : IConverter<T1, T2>
{
    protected IConverter<T1, T2> _converter { get; }

    public Converter(IConverter<T1, T2> converter)
    {
        _converter = converter;
    }

    public T2 Convert(T1 instance)
    {
        return _converter.Convert(instance);
    }

    public T1 Convert(T2 instance)
    {
        throw new NotImplementedException();
    }

    public IQueryResult<T2> Adapt(IQueryResult<T1> queryResult)
    {
        var data = new List<T2>();

        foreach (var item in queryResult.Data)
        {
            data.Add(Convert(item));
        }

        return new QueryResult<T2>(data, queryResult.Pagination);
    }

    public IQueryResult<T1> Adapt(IQueryResult<T2> queryResult)
    {
        var data = new List<T1>();

        foreach (var item in queryResult.Data)
        {
            data.Add(Convert(item));
        }

        return new QueryResult<T1>(data);
    }
}
