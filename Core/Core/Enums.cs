namespace ModularSystem.Core;

public enum EnvironmentType
{
    Development,
    Testing,
    Production,
}

// types of access to a resource
// defines the required permissions
public enum DataAccessType
{
    Read,
    Create,
    Update,
    Delete
}

public enum OperationType
{
    Read,
    Write,
    Delete
}

// types of permissions for a resource access
// defines the required permissions *scope*

public enum DataAccessScope
{
    Self,
    Thirdparty,
    Full,
    Public
}

public enum Ordering
{
    Ascending,
    Descending
}

public enum PriorityLevel : int
{
    Low = 1,
    BelowNormal = 2,
    Normal = 3,
    AboveNormal = 4,
    High = 5
}