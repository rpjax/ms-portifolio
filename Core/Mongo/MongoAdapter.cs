using ModularSystem.Core;
using MongoDB.Bson;

namespace ModularSystem.Mongo;

public abstract class MongoLayerAdapter<InputT, OutputT> : LayerAdapter<InputT, OutputT>
{
    protected MongoLayerAdapter(ILayerAdapter<InputT, OutputT> adapter) : base(adapter)
    {
    }

    public ObjectId TryParseId(string id)
    {
        if (ObjectId.TryParse(id, out ObjectId value))
        {
            return value;
        }

        return ObjectId.Empty;
    }
}