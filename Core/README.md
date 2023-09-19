# ModularSystem Core Module *(Under Development)*

## _Build a Solid Basic CRUD Within Minutes_

This library streamlines the foundational tasks of setting up web servers. ModularSystem aims to automate repetitive aspects of web application development in a modular and simple manner.

## Overview

ModularSystem provides a collection of useful classes for daily development tasks. The core philosophy is to maintain a stable API model to facilitate communication between different servers without requiring specific adaptation layers. This enables parsing and understanding the interface exposed by a server without data mapping, typing, or adaptation. The term 'Modular' reflects the system's extensibility, allowing developers to add modules to enhance functionality and promote code reuse.

## CRUD Operations

The library offers complete CRUD (Create, Read, Update, Delete) operations, featuring a well-defined query mechanism based on Expression trees.

## Generics

The entity interface employs a generic type `T`, which inherits from a base class. This design allows the library to apply dynamic CRUD operations to any class. Developers will need to implement or override specific methods, where application-specific logic like validation and presentation can be added.

## Entity Interface

The Entity Interface serves as the foundation for CRUD operations. It consolidates validation logic, data access layers, and much more to expose methods through the entity.

### Initializing the Library

For the library to function correctly, it's essential to initialize it before use. 
This step ensures that all modules and components are properly set up and ready for use.

#### Step-by-step Guide:

* Import the necessary namespace: 
    Add the ModularSystem.Core namespace to your application.
  
* Call the Initializer:
    Use the Initializer.Run() method to initialize the library.

Example:
```
using ModularSystem.Core;

namespace MyApp;

public static class Program
{
    public static void Main(string[] args)
    {
        Initializer.Run();
    }
}
```

### Getting Started

* To begin, create the data structure for the entity model. This model should implement the `IQueryableModel` interface. Extend from `QueryableModel` to      automatically implement all required methods.

Example:
```
using ModularSystem.Core;

namespace MyApp;

public class User : QueryableModel
{
    public string Email { get; set; }
    public string Password { get; set; }
}
```

* Implement the IEntity<T> interface in the entity class. One approach is to extend from Entity<T> and implement the abstract methods.

Example:
```
using ModularSystem.Core;

namespace MyApp;
  
public class UserEntity : Entity<User>
{
    // ...
}
```
* Now call the entity anywhere to create a usecase.

Example:
```
namespace MyApp;
  
public class Program
{
    public static async Task Main()
    {
        using var userEntity = new UserEntity();
        
        var user = new User() 
        { 
            Email = "foo@bar.baz", 
            Password = "super-password" 
        };
        
        var userId = await entity.CreateAsync(user);
    }
}
```

##
Note that the raw entity class requires implementation of certain methods and properties:

Example:
```
using ModularSystem.Core;

namespace MyApp;

public class UserEntity : Entity<User>
{
    protected IDataAccessObject<User> DataAccessObject { get; }

    public UserEntity()
    {
        //...
    }
    
    protected override MemberExpression CreateIdSelectorExpression  (ParameterExpression parameter)
    {
        //...
    }
    
    protected override object? TryParseId(string id)
    {
        //...
    }
}
```

The library also provides an Entity Framework implementation, with additional modules offering other implementations.

Example:
```
using ModularSystem.Core;
using ModularSystem.EntityFramework;
    
namespace MyApp;

public class UserEntity : EFEntity<User>
{
    protected IDataAccessObject<User> DataAccessObject { get; }
    
    public UserEntity()
    {
        //...
    }
}
```
    
## Data Access Object Interface (IDAO)
The IDataAccessObject interface houses the code for database access, serving as the entity's I/O interface for a given resource. The core library includes embedded implementations like EFCoreDataAccessObject.

Example:
```
using ModularSystem.Core;
using ModularSystem.EntityFramework;

namespace MyApp;

class UserEntity : EFEntity<User>
{
    protected IDataAccessObject<User> DataAccessObject { get; }

    public UserEntity()
    {
        DataAccessObject = new EFCoreDataAccessObject<User>(new MyDbContext());
    }
}
```

## IValidator<T> Interface
Here you define data validation rules for specific data structures. Implement a validation method to return or throw exceptions if the data is in an invalid state.

Example:
```
using ModularSystem.Core;
using ModularSystem.EntityFramework;

namespace MyApp;

public class UserValidator : IValidator<User>
{
    public Task<Exception?> ValidateAsync(User instance)
    {
        //...
    }
}

class UserEntity : EFEntity<User>
{
    protected IDataAccessObject<User> DataAccessObject { get; }

    public UserEntity()
    {
        DataAccessObject = new EFCoreDataAccessObject<User>(new MyDbContext());
        Validator = new UserValidator();
    }
}
```

## Wiring it All Together
Entities can be utilized by other layers to create use cases, thus enabling clean and desired code behavior.

Example:
```
using ModularSystem.Core;

namespace MyApp;

public class MyUseCase
{
    public async Task DoSomeStuff()
    {
        using var entity = new UserEntity();
        var user = new User();
        var id = await entity.CreateAsync(user);
    }

    public async Task DoSomeMoreStuff()
    {
        using var entity = new UserEntity();

        var query = new Query<User>()
            .SetFilter(user => user.Email == "foo@bar.baz");

        var queryResult = await entity.QueryAsync(query);
    }
}
```

## API Controller
Create a basic CRUD API with the ApiController base class, which generates GET, POST, PUT, and DELETE endpoints.

Example:
```
using ModularSystem.Core;
using ModularSystem.Web;

namespace MyApp;

[Route("api/user")]
public class UserController : CrudController<User>
{
    protected override IEntity<User> Entity { get; }

    public UserController() : base()
    {
        Entity = new UserEntity();
    }
}
```

## Accessing the API with a CRUD Client from another C# application.

You can access the API created by the CrudController through an instance of CrudClient<T>.


Note: For seamless communication, it's crucial that both applications either reference the same resource assembly or have exact replicas of it. In this context, the `User` class is the shared resource. It's imperative that the shared resource has a matching type fullname. For instance, the class `MyApp.User` should be present in both applications, even if they reside in separate assemblies or originate from different version sources.

Example (client app):
```
using ModularSystem.Core;
using ModularSystem.Web;
using MyApp;

namespace MyClientApp;

public class Program
{
    public static async Task Main()
    {
        var config = new EndpointConfiguration("https://localhost:5001/api/user");
        var userClient = new CrudClient<User>(config);
        
        var query = new SerializedQueryFactory<User>()
            .SetFilter(user => user.Email == "foo@bar.baz")
            .Create();
            
        var queryResult = await userClient.QueryAsync(query);
    }
}
```

## WORK IN PROGRESS...