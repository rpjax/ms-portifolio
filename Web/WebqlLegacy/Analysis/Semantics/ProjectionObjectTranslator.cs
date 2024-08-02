using Aidan.Core;
using Aidan.Core.Reflection;
using System.Linq.Expressions;

namespace Aidan.Webql.Synthesis;

/// <summary>
/// Constructs a projection for a WebQL query by building an anonymous type. <br/>
/// This class is responsible for interpreting projection expressions within a WebQL query <br/> 
/// and translating them into an anonymous type with corresponding properties and expressions. 
/// </summary>
/// <remarks>This is a translation/synthesis tool.</remarks>
public class ProjectionObjectTranslator : TranslatorBase
{
    public ProjectionObjectTranslator(TranslationOptions options) : base(options)
    {
    }

    /// <summary>
    /// Runs the projection building process on a given ObjectNode within a translation context. <br/>
    /// This method analyzes and translates each property in the object node, constructing a dynamic type for the projection. <br/>
    /// The dynamic type represents the schema of the projected data, with its structure determined by the analysis of the ObjectNode.
    /// </summary>
    /// <param name="context">The translation context for the projection, providing necessary information and tools for translating the projection expression.</param>
    /// <param name="node">The ObjectNode representing the projection expression in the WebQL query. This node contains the structure and expressions defining the projection.</param>
    /// <returns>A Type instance representing the dynamically constructed projection.<br/>
    /// This type can be used to instantiate objects that conform to the projected data structure, <br/>
    /// allowing for dynamic data shaping according to the projection defined in the WebQL query.</returns>
    public ProjectionExpression TranslateProjectionObject(TranslationContextOld context, ObjectNode node)
    {
        var propertyDefinitions = new List<AnonymousPropertyDefinition>();
        var expressions = new List<Expression>();
        var projectedType = null as Type;

        // Itera sobre cada propriedade na expressão de projeção
        foreach (var item in node.Expressions)
        {
            var lhs = item.Lhs.Value;
            var rhs = item.Rhs.Value;

            var isSubTypeDefinition = 
                rhs is ObjectNode child
                && child.IsNotEmpty()
                && !child.First().Lhs.IsOperator;

            if (isSubTypeDefinition)
            {
                var propertyBuilder = new ProjectionObjectTranslator(Options);
                var propertyBindings = new List<MemberBinding>();

                var projectionExpression = new ProjectionObjectTranslator(Options)
                    .TranslateProjectionObject(context, rhs.As<ObjectNode>());
    
                var propertyName = lhs;

                var childPropertyType = projectionExpression.Type;
                var childPropertyDefinitions = projectionExpression.PropertyDefinitions;
                var childExpressions = projectionExpression.Expressions;

                for (int i = 0; i < childPropertyDefinitions.Length; i++)
                {
                    var propDefinition = childPropertyDefinitions[i];
                    var childExpression = childExpressions[i];

                    var propertyInfo = childPropertyType.GetProperty(propDefinition.Name);

                    if (propertyInfo == null)
                    {
                        throw new TranslationException($"Property '{propDefinition.Name}' could not be found in the type '{childPropertyType.FullName}'. This might indicate an inconsistency in the projection definition.", context);
                    }

                    // Cria um binding para a propriedade do novo tipo
                    propertyBindings.Add(Expression.Bind(propertyInfo, childExpression));
                }

                // Cria a expressão 'new projectedType { Prop1 = ..., Prop2 = ..., ... }'
                var expression = Expression.MemberInit(Expression.New(childPropertyType), propertyBindings);

                propertyDefinitions.Add(new AnonymousPropertyDefinition(propertyName, childPropertyType));
                expressions.Add(expression);
            }
            else
            {
                if (item.Lhs.IsOperator)
                {
                    throw new TranslationException($"The projection property '{lhs}' is invalid because it uses an operator. In WebQL projections, the root of the projection must exclusively contain property definitions that map directly to fields, define sub-objects, or specify expressions without operators at the root level. This is because the projection root describes the projected data structure and must correspond to property names and their associated values or sub-structures. For example, 'name': '$username' directly maps to a field, 'address': {{'street': '$streetName' }} defines a sub-object, and 'status': {{'$select': 'isActive', '$equals': true }} specifies a valid expression within a sub-object. Please adjust the projection definition to ensure it only contains valid property mappings.", context);
                }

                // Obtém o nome da propriedade e a expressão associada
                var propertyName = item.Lhs.Value;
                var literalNode = TypeCastNode<LiteralNode>(context, item.Rhs.Value);

                var propertyExpression = TranslateReference(context, literalNode); 

                propertyDefinitions.Add(new AnonymousPropertyDefinition(propertyName, propertyExpression.Type));
                expressions.Add(propertyExpression);
            }
        }

        if (propertyDefinitions.Count == 0)
        {
            throw new TranslationException("No properties were defined in the projection. A valid projection must contain at least one property. Please review your query structure.", context);
        }

        var options = new AnonymousTypeCreationOptions()
        {
            Properties = propertyDefinitions,
            CreateDefaultConstructor = true,
            CreateSetters = true
        };

        projectedType = TypeCreator.CreateAnonymousType(options);

        if (projectedType == null)
        {
            throw new TranslationException("Failed to create projected type: Anonymous type generation encountered an internal error. Review the property definitions for any inconsistencies.", context);
        }

        return new ProjectionExpression(propertyDefinitions, expressions, projectedType);
    }

    public Expression TranslateProjectionObject2(TranslationContextOld context, ObjectNode node)
    {
        var propertyDefinitions = new List<AnonymousPropertyDefinition>();
        var mainpropertyBindings = new List<MemberBinding>();
        var projectedType = null as Type;
        throw new NotImplementedException();
        //// Itera sobre cada propriedade na expressão de projeção
        //foreach (var item in node.Expressions)
        //{
        //    var lhs = item.Lhs.Value;
        //    var rhs = item.Rhs.Value;

        //    var isSubTypeDefinition =
        //        rhs is ObjectNode child
        //        && child.IsNotEmpty()
        //        && !child.First().Lhs.IsOperator;

        //    if (isSubTypeDefinition)
        //    {
        //        var propertyBuilder = new ProjectionObjectTranslator(Options);
        //        var propertyBindings = new List<MemberBinding>();

        //        var projectionExpression = new ProjectionObjectTranslator(Options)
        //            .TranslateProjectionObject(context, rhs.As<ObjectNode>());

        //        var propertyName = lhs;

        //        var childPropertyType = projectionExpression.Type;
        //        var childPropertyDefinitions = projectionExpression.PropertyDefinitions;
        //        var childExpressions = projectionExpression.Expressions;

        //        for (int i = 0; i < childPropertyDefinitions.Length; i++)
        //        {
        //            var propDefinition = childPropertyDefinitions[i];
        //            var childExpression = childExpressions[i];

        //            var propertyInfo = childPropertyType.GetProperty(propDefinition.Name);

        //            if (propertyInfo == null)
        //            {
        //                throw new TranslationException($"Property '{propDefinition.Name}' could not be found in the type '{childPropertyType.FullName}'. This might indicate an inconsistency in the projection definition.", context);
        //            }

        //            // Cria um binding para a propriedade do novo tipo
        //            propertyBindings.Add(Expression.Bind(propertyInfo, childExpression));
        //        }

        //        // Cria a expressão 'new projectedType { Prop1 = ..., Prop2 = ..., ... }'
        //        var expression = Expression.MemberInit(Expression.New(childPropertyType), propertyBindings);

        //        propertyDefinitions.Add(new AnonymousPropertyDefinition(propertyName, childPropertyType));
        //        mainpropertyBindings.Add(expression);
        //    }
        //    else
        //    {
        //        if (item.Lhs.IsOperator)
        //        {
        //            throw new TranslationException($"The projection property '{lhs}' is invalid because it uses an operator. In WebQL projections, the root of the projection must exclusively contain property definitions that map directly to fields, define sub-objects, or specify expressions without operators at the root level. This is because the projection root describes the projected data structure and must correspond to property names and their associated values or sub-structures. For example, 'name': '$username' directly maps to a field, 'address': {{'street': '$streetName' }} defines a sub-object, and 'status': {{'$select': 'isActive', '$equals': true }} specifies a valid expression within a sub-object. Please adjust the projection definition to ensure it only contains valid property mappings.", context);
        //        }

        //        // Obtém o nome da propriedade e a expressão associada
        //        var propertyName = item.Lhs.Value;
        //        var literalNode = TypeCastNode<LiteralNode>(context, item.Rhs.Value);

        //        var propertyExpression = TranslateReference(context, literalNode);

        //        propertyDefinitions.Add(new AnonymousPropertyDefinition(propertyName, propertyExpression.Type));
        //        mainpropertyBindings.Add(propertyExpression);
        //    }
        //}

        //if (propertyDefinitions.Count == 0)
        //{
        //    throw new TranslationException("No properties were defined in the projection. A valid projection must contain at least one property. Please review your query structure.", context);
        //}

        //var options = new AnonymousTypeCreationOptions()
        //{
        //    Properties = propertyDefinitions,
        //    CreateDefaultConstructor = true,
        //    CreateSetters = true
        //};

        //projectedType = TypeCreator.CreateAnonymousType(options);

        //if (projectedType == null)
        //{
        //    throw new TranslationException("Failed to create projected type: Anonymous type generation encountered an internal error. Review the property definitions for any inconsistencies.", context);
        //}

        //// Cria a expressão 'new projectedType { Prop1 = ..., Prop2 = ..., ... }'
        //var newExpression = Expression.MemberInit(Expression.New(projectedType), mainpropertyBindings);

        //return new ProjectionExpression(propertyDefinitions, mainpropertyBindings, projectedType);
    }

}

public class ProjectionExpression : Expression
{
    public AnonymousPropertyDefinition[] PropertyDefinitions { get; }
    public Expression[] Expressions { get; } 
    public Type Type { get;  } 

    public ProjectionExpression(
        IEnumerable<AnonymousPropertyDefinition> propertyDefinitions,
        IEnumerable<Expression> expressions, 
        Type type
    )
    {
        PropertyDefinitions = propertyDefinitions.ToArray();
        Expressions = expressions.ToArray();
        Type = type;
    }
}
