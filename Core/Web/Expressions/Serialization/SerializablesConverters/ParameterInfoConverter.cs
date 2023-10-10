using ModularSystem.Core;
using System.Reflection;

namespace ModularSystem.Web.Expressions;

public interface IParameterInfoConverter : IConverter<ParameterInfo, SerializableParameterInfo>
{

}

public class ParameterInfoConverter : IParameterInfoConverter
{
    public SerializableParameterInfo Convert(ParameterInfo instance)
    {
        return new SerializableParameterInfo()
        {

        };
    }

    public ParameterInfo Convert(SerializableParameterInfo instance)
    {
        throw new NotImplementedException();
    }
}