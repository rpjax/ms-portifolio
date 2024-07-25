namespace ModularSystem.Core.Patterns;

/// <summary>
/// Marker interface for explicitly identifying entities within the system. 
/// <br/>
/// This interface supports basic comparison mechanics through <see cref="IEquatable{IEntity}"/>.
/// </summary>
public interface IEntity : IEquatable<IEntity>
{
    
}
