using ModularSystem.Core;
using ModularSystem.Core.Reflection;
using ModularSystem.Webql.Synthesis;
using System.Linq.Expressions;

namespace ModularSystem.Webql.Analysis;

/// <summary>
/// Constructs a projection for a WebQL query by building an anonymous type. <br/>
/// This class is responsible for interpreting projection expressions within a WebQL query <br/> 
/// and translating them into an anonymous type with corresponding properties and expressions. 
/// </summary>
/// <remarks>This is a translation/synthesis tool.</remarks>
public class ProjectionBuilder
{
    /// <summary>
    /// A collection of anonymous property definitions that represent the structure of the projected type.
    /// </summary>
    public List<AnonymousPropertyDefinition> Properties { get; private set; } = new();

    /// <summary>
    /// A collection of expressions corresponding to the properties in the projected type.
    /// </summary>
    public List<Expression> Expressions { get; set; } = new();

    /// <summary>
    /// The resulting projected type created by the builder, or null if the projection has not been built.
    /// </summary>
    public Type? ProjectedType { get; private set; } = null;

    /// <summary>
    /// The translator used for converting WebQL nodes into LINQ expressions.
    /// </summary>
    private NodeTranslator Translator { get; }

    /// <summary>
    /// Initializes a new instance of the ProjectionBuilder class.
    /// </summary>
    /// <param name="translator">The NodeTranslator instance used for translating WebQL nodes.</param>
    public ProjectionBuilder(NodeTranslator translator)
    {
        Translator = translator;
    }

    /// <summary>
    /// Runs the projection building process on a given ObjectNode within a translation context.
    /// This method analyzes and translates each property in the object node, constructing a dynamic type for the projection.
    /// </summary>
    /// <param name="context">The translation context for the projection.</param>
    /// <param name="node">The ObjectNode representing the projection expression in the WebQL query.</param>
    /// <returns>The ProjectionBuilder instance with the constructed projection.</returns>
    public ProjectionBuilder Run(TranslationContext context, ObjectNode node)
    {
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
                var propertyBindings = new List<MemberBinding>();
                var propertyBuilder = new ProjectionBuilder(Translator)
                    .Run(context, rhs.As<ObjectNode>());

                var propertyName = lhs;
                var propertyType = propertyBuilder.ProjectedType!;

                for (int i = 0; i < propertyBuilder.Properties.Count; i++)
                {
                    var propDefinition = propertyBuilder.Properties[i];
                    var propertyExpression = propertyBuilder.Expressions[i];

                    var propertyInfo = propertyType.GetProperty(propDefinition.Name);

                    if (propertyInfo == null)
                    {
                        throw new TranslationException($"Property '{propDefinition.Name}' could not be found in the type '{propertyType.FullName}'. This might indicate an inconsistency in the projection definition.", context);
                    }

                    // Cria um binding para a propriedade do novo tipo
                    propertyBindings.Add(Expression.Bind(propertyInfo, propertyExpression));
                }

                // Cria a expressão 'new projectedType { Prop1 = ..., Prop2 = ..., ... }'
                var expression = Expression.MemberInit(Expression.New(propertyType), propertyBindings);

                Properties.Add(new(propertyName, propertyType));
                Expressions.Add(expression);
            }
            else
            {
                if (item.Lhs.IsOperator)
                {
                    throw new TranslationException("The projection property '" + lhs + "' contains an operator, which is not allowed in this context. In WebQL projections, each property should either map directly to a field or define a sub-object or expression. For example, 'name': '$username' is a direct field binding, 'address': { 'street': '$streetName' } defines a sub-object, and 'status': { '$select': 'isActive', '$equals': true } is a valid expression. Ensure the property definition adheres to these rules.", context);
                }

                // Obtém o nome da propriedade e a expressão associada
                var propertyName = item.Lhs.Value;
                var propertyExpression = Translator.Translate(context, item.Rhs.Value);

                Properties.Add(new(propertyName, propertyExpression.Type));
                Expressions.Add(propertyExpression);
            }
        }

        if (Properties.Count == 0)
        {
            throw new TranslationException("No properties were defined in the projection. A valid projection must contain at least one property. Please review your query structure.", context);
        }

        var options = new AnonymousTypeCreationOptions()
        {
            CreateDefaultConstructor = true,
            CreateSetters = true
        };

        ProjectedType = TypeCreator.CreateAnonymousType(Properties, options);

        if (ProjectedType == null)
        {
            throw new TranslationException("Failed to create projected type: Anonymous type generation encountered an internal error. Review the property definitions for any inconsistencies.", context);
        }

        return this;
    }

}
