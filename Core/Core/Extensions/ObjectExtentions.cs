using System.Reflection;

namespace ModularSystem.Core;

public static partial class ObjectExtentions
{
    public class Variance
    {
        public string Property { get; set; }
        public object? ValueA { get; set; }
        public object? ValueB { get; set; }

        public Variance(string prop, object? a, object? b)
        {
            Property = prop;
            ValueA = a;
            ValueB = b;
        }
    }

    public static T? TryTypeCast<T>(this object value) where T : class
    {
        try
        {
            var cast = value as T;

            if (cast != null)
            {
                return cast;
            }

            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static T TypeCast<T>(this object value)
    {
        try
        {
            return (T)value;
        }
        catch (Exception e)
        {
            throw new Exception($"Could not cast object to type '{value.GetType().FullName}'.", e);
        }
    }

    public static Variance[] PropertyCompare<T>(this T objectA, T? objectB) where T : class
    {
        if (objectB == null)
        {
            return new Variance[0];
        }

        List<Variance> variances = new List<Variance>();
        FieldInfo[] fileInfo = objectA.GetType().GetFields();

        foreach (FieldInfo f in fileInfo)
        {
            var name = f.Name;
            var valueA = f.GetValue(objectA);
            var valueB = f.GetValue(objectB);

            if (!Equals(valueA, valueB))
            {
                variances.Add(new Variance(name, valueA, valueB));
            }
        }

        return variances.ToArray();
    }
}

