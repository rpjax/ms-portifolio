namespace ModularSystem.Web.Expressions;

public interface IMapper<InT, OutT>
{
    OutT? Get(InT data);

    void Map(InT source, OutT target);
}