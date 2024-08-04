# WebQL v3.0.0 - Language Documentation
## Introduction

WebQL is a powerful query language designed to be compiled into LINQ expressions, allowing seamless integration with LINQ providers in C#. This documentation will guide you through the usage of WebQL, covering its syntax, semantics, operators, and providing comprehensive examples to get you started.

## Table of Contents
// fix this markdown reference
1. [Getting Started](#getting-started)
2. [Basic Syntax](#basic-syntax)
3. [Literals](#literals)
4. [Expressions](#expressions)
   1. [Literal Expressions](#literal-expressions)
   2. [Reference Expressions](#reference-expressions)
   3. [Scope Access Expressions](#scope-access-expressions)
   4. [Block Expressions](#block-expressions)
5. [Operators](#operators)
   1. [Arithmetic Operators](#arithmetic-operators)
   2. [Relational Operators](#relational-operators)
   3. [Logical Operators](#logical-operators)
   4. [String Relational Operators](#string-relational-operators)
   5. [Collection Manipulation Operators](#collection-manipulation-operators)
   6. [Collection Aggregation Operators](#collection-aggregation-operators)
6. [Examples](#examples)

## Getting Started

### Importing the Library
To start using WebQL in your project, you need to integrate the WebQL compiler. This compiler is available under the namespace `Webql`.

#### Import Statement
```csharp
using Webql;
```

### Using the Compiler

#### Basic Setup
Here is a simple example to get you started:

```csharp
using Webql;

namespace MyProject;

public static void Program 
{
    public static void Main()
    {
        var compiler = new WebqlCompiler();
        //...
    }
}
```

Configuring the Compiler
For more advanced usage, you can customize the compiler settings by providing an instance of `WebqlCompilerSettings`. This allows you to tailor the compiler behavior to suit your specific needs.

```csharp
using Webql;

namespace MyProject;

public static void Program 
{
    public static void Main()
    {
        var settings = new WebqlCompilerSettings(
            // Configure your options here...
        );

        var compiler = new WebqlCompiler(settings);
        //...
    }
}
```

### Compiling a Query

Once you have set up the compiler, you can start writing and compiling queries. Below is an example of how to compile a WebQL query into a LINQ Expression:

```csharp
public static void Main(string[] args)
{
    var compiler = new WebqlCompiler();
    
    var query = "...";
    var elementType = typeof(...);

    var expression = compiler.Compile(query, elementType);

    //...
}
```

## Writing Queries

Before you start writing queries its important to understand some key points about the language semantics. A WebQL query can be thought of as a function that takes a data collection and returns some result.

#### Example:
``` csharp
public TResult CreateQuery(IQueryable<TElement> source)
{
    ...
}
```

In the above example, `TElement` is the type of the elements in the data collection, and `TResult` is the type of the result returned by the query.
