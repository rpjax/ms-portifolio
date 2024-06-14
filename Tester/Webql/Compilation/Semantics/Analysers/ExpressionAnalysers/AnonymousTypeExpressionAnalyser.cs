using ModularSystem.Core.Reflection;
using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Analysers;

public static class AnonymousTypeExpressionAnalyser
{
    public static TypeProjectionExpressionSemantic AnalyseAnonymousTypeExpression(
        SemanticContextOld context,
        AnonymousTypeExpressionSymbol symbol)
    {
        var properties = new List<AnonymousPropertyDefinition>();
        var options = new AnonymousTypeCreationOptions()
        {
            Properties = properties,
            CreateDefaultConstructor = true,
            CreateSetters = true,
            UseCache = true
        };

        foreach (var binding in symbol)
        {
            properties.Add(ResolveBinding(context, binding));
        }

        var type = TypeCreator.CreateAnonymousType(options);

        return new TypeProjectionExpressionSemantic(
            type: type
        );
    }

    private static AnonymousPropertyDefinition ResolveBinding(SemanticContextOld context, TypeBindingSymbol binding)
    {
        var name = binding.Name;
        var value = binding.Value;

        var semantic = SemanticAnalyser.AnalyseExpression(
            context: context.GetSymbolContext(value), 
            symbol: value
        );

        return new AnonymousPropertyDefinition(
            name: name, 
            type: semantic.Type
        );
    }

}
