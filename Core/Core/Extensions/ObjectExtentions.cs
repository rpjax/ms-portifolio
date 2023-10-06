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

    /// <summary>
    /// Attempts to safely cast the given object to the specified type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The target type to which the object should be casted. Must be a reference type.</typeparam>
    /// <param name="value">The object to be casted.</param>
    /// <returns>
    /// The casted object if the cast is successful; otherwise, null. <br/>
    /// Note that if the provided object is not of the type <typeparamref name="T"/>, this method will return null 
    /// rather than throwing an exception.
    /// </returns>
    public static T? TryTypeCast<T>(this object value) where T : class
    {
        return value as T;
    }

    /// <summary>
    /// Casts the given object to the specified type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The target type to which the object should be casted.</typeparam>
    /// <param name="value">The object to be casted.</param>
    /// <returns>The casted object of type <typeparamref name="T"/>.</returns>
    /// <exception cref="Exception">
    /// Throws an exception if the cast fails, providing details about the type of the original object.
    /// </exception>
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

