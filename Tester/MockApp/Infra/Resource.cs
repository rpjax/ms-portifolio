//using ModularSystem.Core;

//namespace ModularSystem.Tester;

//public class CoinResource : Resource<Coin>
//{
//    public override Entity<Coin> Entity { get; }

//    public CoinResource(ResourcePolicySet resourcePolicy, string key) : base(resourcePolicy, key)
//    {
//        Entity = new CoinEntity(key);
//    }

//    public Task<Coin> ConvertAsync(Coin data)
//    {
//        var pipe = CreatePipeline<Request, Coin>();

//        pipe.Append(new ActionPipeline<Request, Coin>((req) =>
//        {
//            return Entity.GetAsync("");
//        }));

//        return pipe.RunAsync(new StringRequest(RequestContext.Public(), ""));
//    }
//}

//public class PaperResource : Resource<Paper>
//{
//    public PaperResource(ResourcePolicySet policyPreset, string masterKey = "") : base(policyPreset, masterKey)
//    {
//        Entity = new PaperEntity();
//    }

//    public override IEntity<Paper> Entity { get; }
//}
