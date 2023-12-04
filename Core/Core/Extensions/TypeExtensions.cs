﻿using System.Reflection.Emit;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ModularSystem.Core;

/// <summary>
/// Provides extension methods for the .NET System.Type class, offering useful functionalities for working with types and type hierarchies at runtime.
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// Checks whether a child type is a subtype of a parent type.
    /// </summary>
    /// <param name="childType">The type that might be a subtype.</param>
    /// <param name="parentType">The type that might be a supertype.</param>
    /// <returns>True if the childType is a subtype of parentType, otherwise false.</returns>
    /// <remarks>
    /// Considers generic types, ensuring that a List of int is not considered equal to a List of string, for example.
    /// </remarks>
    public static bool IsSubtypeOf(this Type childType, Type parentType)
    {
        Type? iteratorType = childType;

        while (iteratorType != null && iteratorType != typeof(object))
        {
            Type type;

            //*
            // this ensures that List<int> is not equal to List<string>.
            //*
            if (iteratorType.IsGenericType && parentType.IsGenericType)
            {
                type = iteratorType.GetGenericTypeDefinition();
            }
            else
            {
                type = iteratorType;
            }

            if (parentType == type)
            {
                return true;
            }

            iteratorType = iteratorType.BaseType;
        }

        return false;
    }

    /// <summary>
    /// Determines whether the specified type implements the specified interface.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <param name="interfaceType">The interface to check against.</param>
    /// <returns>True if the type implements the interface, otherwise false.</returns>
    /// <remarks>
    /// Works with generic interfaces.
    /// </remarks>
    public static bool ImplementsInterface(this Type type, Type interfaceType)
    {
        foreach (var implementedInterface in type.GetInterfaces())
        {
            if (implementedInterface == interfaceType ||
                (interfaceType.IsGenericType && implementedInterface.IsGenericType &&
                implementedInterface.GetGenericTypeDefinition() == interfaceType))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Generic version of IsSubtypeOf, facilitating checks against a specific parent type.
    /// </summary>
    /// <param name="childType">The type that might be a subtype.</param>
    /// <returns>True if the childType is a subtype of T, otherwise false.</returns>
    public static bool IsSubtypeOf<T>(this Type childType)
    {
        return IsSubtypeOf(childType, typeof(T));
    }

    /// <summary>
    /// Checks if the source type is an instance of a generic type matching the target type.
    /// </summary>
    /// <param name="source">The source type to check.</param>
    /// <param name="target">The target generic type.</param>
    /// <returns>True if the source type is an instance of the target generic type, otherwise false.</returns>
    public static bool IsInstanceOfGenericType(this Type source, Type target)
    {
        Type? _type = source;

        while (_type != null && _type != typeof(object))
        {
            if (_type.IsGenericType && _type.GetGenericTypeDefinition() == target)
                return true;

            _type = _type.BaseType;
        }

        return false;
    }

    /// <summary>
    /// Generic version of IsInstanceOfGenericType.
    /// </summary>
    /// <param name="source">The source type to check.</param>
    /// <returns>True if the source type is an instance of T, otherwise false.</returns>
    public static bool IsInstanceOfGenericType<T>(this Type source)
    {
        return source.IsInstanceOfGenericType(typeof(T));
    }

    public static int? InheritanceDistance(this Type baseType, Type derivedType)
    {
        int distance = 0;

        Type? targetType = derivedType;

        while (targetType != null && targetType != baseType)
        {
            targetType = targetType.BaseType;
            distance++;
        }

        if (derivedType == baseType)
        {
            return distance;
        }

        return null; // means derivedType is not derived from baseType
    }

    public static bool IsNullable(this Type type)
    {
        return Nullable.GetUnderlyingType(type) != null;
    }

    /// <summary>
    /// Retrieves a specific method from a type based on the given method name, parameter types, and return type.
    /// </summary>
    /// <param name="self">The type from which to retrieve the method.</param>
    /// <param name="name">The name of the method to retrieve.</param>
    /// <param name="parameters">An array of types representing the parameters of the method.</param>
    /// <param name="outputType">The return type of the method.</param>
    /// <returns>
    /// The <see cref="MethodInfo"/> object representing the method that matches the specified criteria;
    /// returns null if no such method is found.
    /// </returns>
    /// <exception cref="AmbiguousMatchException">
    /// Thrown when multiple methods match the specified criteria, making the method call ambiguous.
    /// </exception>
    /// <remarks>
    /// This method is useful for retrieving methods in cases where multiple overloads may exist,
    /// and a specific method needs to be obtained based on its signature.
    /// </remarks>
    public static MethodInfo? GetMethod(this Type self, string name, Type[] parameters, Type outputType)
    {
        var methods = self
            .GetMethods()
            .Where(x => x.Name == name)
            .Where(x => x.ReturnType == outputType)
            .Where(x => parameters.All(paramType => x.GetParameters().Any(methodParam => methodParam.ParameterType == paramType)))
            .ToArray();

        if (methods.Length == 0)
        {
            return null;
        }
        if (methods.Length > 1)
        {
            throw new AmbiguousMatchException($"Multiple methods found matching the name '{name}', parameter types, and return type. Please specify more distinct criteria.");
        }

        return methods[0];
    }
}

public class AnonymousPropertyDefinition
{
    public string Name { get; }
    public Type Type { get; }

    public AnonymousPropertyDefinition(string name, Type type)
    {
        Name = name;
        Type = type;
    }
}

public class AnonymousTypeCreationOptions
{
    public string Name { get; set; } = "<>f__AnonymousType";
    public bool CreateDefaultConstructor { get; set; } = false;
    public bool CreateSetters { get; set; } = false;
}

/// <summary>
/// Provides functionalities for dynamic type creation, including the generation of anonymous types.
/// </summary>
public static class TypeHelper
{
    /// <summary>
    /// Dynamically creates an anonymous type based on the provided properties. This method allows for the creation of types with customizable properties, including the option to have setters and a default constructor.
    /// </summary>
    /// <param name="properties">An array of <see cref="AnonymousPropertyDefinition"/> that defines the properties the anonymous type should have.</param>
    /// <param name="options">An instance of <see cref="AnonymousTypeCreationOptions"/> that specifies options such as the name of the type, and whether to create setters and a default constructor.</param>
    /// <returns>A new type, created dynamically, that represents the defined anonymous type.</returns>
    /// <remarks>
    /// This method uses <see cref="System.Reflection.Emit.AssemblyBuilder"/> and <see cref="System.Reflection.Emit.ModuleBuilder"/> to create a type at runtime.
    /// It can be used to create types that closely resemble anonymous types created by the C# compiler, with additional flexibility.
    /// </remarks>
    public static Type CreateAnonymousType(AnonymousPropertyDefinition[] properties, AnonymousTypeCreationOptions options = null)
    {
        options ??= new();

        var name = options.Name;
        var createSetters = options.CreateSetters;

        // Cria um assembly dinâmico
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("DynamicAssembly"), AssemblyBuilderAccess.Run);
        var moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicModule");

        // Cria um tipo dinâmico
        var typeBuilder = moduleBuilder.DefineType($"{name}", TypeAttributes.Public);

        // Define os campos
        var fieldBuilders = new FieldBuilder[properties.Length];

        // Defines some data notations attributes that exists in a real anonymous type generated by the compiler.
        typeBuilder.SetCustomAttribute(new CustomAttributeBuilder(
            typeof(CompilerGeneratedAttribute).GetConstructor(Type.EmptyTypes)!, Array.Empty<object>()));

        typeBuilder.SetCustomAttribute(new CustomAttributeBuilder(
            typeof(DebuggerDisplayAttribute).GetConstructor(new[] { typeof(string) })!, new object[] { "{ToString()}" }));


        for (int i = 0; i < properties.Length; i++)
        {
            fieldBuilders[i] = typeBuilder.DefineField($"<{properties[i].Name}>k__BackingField", properties[i].Type, FieldAttributes.Private);
        }

        if (options.CreateDefaultConstructor)
        {
            // default constructor
            var defaultConstructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
            var defaultConstructorGenerator = defaultConstructorBuilder.GetILGenerator();

            defaultConstructorGenerator.Emit(OpCodes.Ret);
        }

        // Define o construtor
        var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, properties.Select(p => p.Type).ToArray());

        var ctorIL = constructorBuilder.GetILGenerator();

        for (int i = 0; i < properties.Length; i++)
        {
            ctorIL.Emit(OpCodes.Ldarg_0); // Carrega o this
            ctorIL.Emit(OpCodes.Ldarg, i + 1); // Carrega o argumento (1-indexado)
            ctorIL.Emit(OpCodes.Stfld, fieldBuilders[i]); // Define o campo
        }

        ctorIL.Emit(OpCodes.Ret);

        // Define as propriedades e seus acessadores
        for (int i = 0; i < properties.Length; i++)
        {
            var propertyBuilder = typeBuilder.DefineProperty(properties[i].Name, PropertyAttributes.HasDefault, properties[i].Type, null);

            // Define o getter
            var getterMethodBuilder = typeBuilder.DefineMethod($"get_{properties[i].Name}", MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, properties[i].Type, Type.EmptyTypes);
            var getterIL = getterMethodBuilder.GetILGenerator();

            getterIL.Emit(OpCodes.Ldarg_0);
            getterIL.Emit(OpCodes.Ldfld, fieldBuilders[i]);
            getterIL.Emit(OpCodes.Ret);
            propertyBuilder.SetGetMethod(getterMethodBuilder);

            if (createSetters)
            {
                var setterMethodBuilder = typeBuilder.DefineMethod($"set_{properties[i].Name}", MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null, new[] { properties[i].Type });
                var setterIL = setterMethodBuilder.GetILGenerator();

                setterIL.Emit(OpCodes.Ldarg_0);
                setterIL.Emit(OpCodes.Ldarg_1);
                setterIL.Emit(OpCodes.Stfld, fieldBuilders[i]);
                setterIL.Emit(OpCodes.Ret);
                propertyBuilder.SetSetMethod(setterMethodBuilder);
            }
        }

        // Cria o tipo
        return typeBuilder.CreateType()!;
    }

}