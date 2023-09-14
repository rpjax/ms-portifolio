namespace ModularSystem.Core;

public class LayerAdapter<InputT, OutputT> : ILayerAdapter<InputT, OutputT>
{
    protected ILayerAdapter<InputT, OutputT> _adapter;

    public LayerAdapter(ILayerAdapter<InputT, OutputT> adapter)
    {
        _adapter = adapter;
    }

    public InputT Adapt(OutputT instance)
    {
        return _adapter.Adapt(instance);
    }

    public OutputT Present(InputT instance)
    {
        return _adapter.Present(instance);
    }

    public IQueryResult<InputT> Adapt(IQueryResult<OutputT> queryResult)
    {
        var data = new List<InputT>();

        foreach (var item in queryResult.Data)
        {
            data.Add(Adapt(item));
        }

        return new QueryResult<InputT>(data);
    }

    public IQueryResult<OutputT> Present(IQueryResult<InputT> queryResult)
    {
        var data = new List<OutputT>();

        foreach (var item in queryResult.Data)
        {
            data.Add(Present(item));
        }

        return new QueryResult<OutputT>(data, queryResult.Pagination);
    }
}