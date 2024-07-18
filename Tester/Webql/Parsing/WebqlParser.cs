using System.Runtime.CompilerServices;
using ModularSystem.Core.TextAnalysis.Gdef;
using ModularSystem.Core.TextAnalysis.Parsing;
using ModularSystem.Core.TextAnalysis.Parsing.Components;
using ModularSystem.Core.TextAnalysis.Parsing.Tools;
using Webql.Parsing.Ast;
using Webql.Parsing.Ast.Builder;

namespace Webql.Parsing;

/// <summary>
/// Parser for the WebQL document syntax (V3).
/// </summary>
public static class WebqlParser
{
    const string RawGrammarText = @"
/*
	Document Syntax for WebQL, Version 3.0.0; Created by Rodrigo Jacques.
	Oficial WebQL repository: https://github.com/rpjcoding/webql-csharp

	NOTE: This document describes syntax using a custom notation, derived from GNU BISON and ANTLR, called GDEF (Grammar Defition Notation).
	For more information about GDEF go to: https://github.com/rpjcoding/gdef

	NOTE: Non-terminals that start with '$' are tokens. Example: $id, $int, $float, $string, etc.
*/

start
	: document 
	;

document
	: expression
	;

/*
	The anonymous_object_expression has been deliberately left out of the expression rule to avoid ambiguity with the block_expression rule.
	The anonymous_object_expression is only meant to be used as a parameter for operators that require a set of properties, such as the $select operator.
*/

expression 
	: literal_expression
	| reference_expression
	| scope_access_expression
	| block_expression
	| operation_expression
	;

literal_expression
	: bool
	| null
	| $int
	| $float
	| $hex
	| $string
	;

bool
	: 'true'
	| 'false'
	;

null
	: 'null'
	;

reference_expression
	: $id
	| '$' $string // legacy support
	;

scope_access_expression
	: reference_expression ':' expression
	;

block_expression
	: '{' [ expression ] { ',' expression } '}' 
	;

operation_expression
	: arithmetic_expression	
	| relational_expression
	| string_relational_expression
	| logical_expression
	| semantic_expression
	| collection_manipulation_expression
	| collection_aggregation_expression
	;

arithmetic_expression
	: add_expression
	| subtract_expression
	| multiply_expression
	| divide_expression
	| modulo_expression
	;

add_expression
	: '$add' ':' expression
	;

subtract_expression
	: '$subtract' ':' expression
	;

multiply_expression
	: '$multiply' ':' expression
	;

modulo_expression
	: '$modulo' ':' expression
	;

divide_expression
	: '$divide' ':' expression
	;

relational_expression
	: equals_expression
	| not_equals_expression
	| greater_expression
	| less_expression
	| greater_equals_expression
	| less_equals_expression
	;

equals_expression
	: '$equals' ':' expression
	;

not_equals_expression
	: '$notEquals' ':' expression
	;

greater_expression
	: '$greater' ':' expression
	;

less_expression
	: '$less' ':' expression
	;

greater_equals_expression
	: '$greaterEquals' ':' expression
	;

less_equals_expression
	: '$lessEquals' ':' expression
	;

string_relational_expression
	: like_expression
	| regex_expression
	;

like_expression
	: '$like' ':' expression
	;

regex_expression
	: '$regex' ':' expression
	;

logical_expression
	: and_expression
	| or_expression
	| not_expression
	;	

and_expression
	: '$and' ':' expression
	;

or_expression
	: '$or' ':' expression
	;

not_expression
	: '$not' ':' expression
	;

semantic_expression
	: aggregate_expression
	;

aggregate_expression
	: '$aggregate' ':' expression
	;

collection_manipulation_expression
	: filter_expression
	| select_expression
	| group_expression
	| order_expression
	| limit_expression
	| skip_expression
	;

filter_expression
	: '$filter' ':' expression
	;

select_expression
	: '$select' ':' anonymous_object_expression
	;

group_expression
	: '$group' ':' expression
	;

order_expression
	: '$order' ':' expression
	;

limit_expression
	: '$limit' ':' $int
	;

skip_expression
	: '$skip' ':' $int
	;

collection_aggregation_expression
	: count_expression
	| contains_expression
	| any_expression
	| all_expression
	| sum_expression
	| average_expression
	| min_expression
	| max_expression
	;

count_expression
	: '$count' ':' expression
	;

contains_expression
	: '$contains' ':' expression
	;

any_expression
	: '$any' ':' expression
	;

all_expression
	: '$all' ':' expression
	;

sum_expression
	: '$sum' ':' expression
	;

average_expression
	: '$average' ':' expression
	;

min_expression
	: '$min' ':' expression
	;

max_expression
	: '$max' ':' expression
	;

anonymous_object_expression
	: '{' anonymous_object_property { ',' anonymous_object_property } '}'
	;

anonymous_object_property
	: $id ':' expression
	| $string ':' expression
	;
";

    private static string[] ReduceWhitelist { get; } = new string[]
    {
		// main constructs
		"document",
        "operator",

		// expressions
        "literal_expression",
        "reference_expression",
        "scope_access_expression",
        "block_expression",
        "operation_expression",
        "anonymous_object_expression",
    };

    private static LR1Parser? ParserInstance { get; set; }

    static WebqlParser()
    {
		// initialize the parser instance
        GetParser();
    }

	/// <summary>
	/// Get the LR1 parser instance for the WebQL syntax grammar.
	/// </summary>
	/// <returns></returns>
    public static LR1Parser GetParser()
    {
        if (ParserInstance is null)
        {
            ParserInstance = new LR1Parser(
                grammar: GdefParser.ParseGrammar(RawGrammarText)
            );
        }

        return ParserInstance;
    }

    /// <summary>
    /// Parse a text into a CST and reduce it into to a more manageable CST.
    /// </summary>
    /// <remarks>
    /// Note that the reduction process does not create an AST, but a CST with a reduced set of nodes. 
    /// <br/>
    /// To create the AST use the <see cref="ParseToAst"/>.
    /// </remarks>
    /// <param name="text"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CstRoot Parse(string text)
    {
        var cst = GetParser().Parse(text);
        var reducer = new CstReducer(cst, ReduceWhitelist);

        return reducer.Execute();
    }

	/// <summary>
	/// Parse a text into an AST according to the WebQL syntax grammar.
	/// </summary>
	/// <param name="text"></param>
	/// <returns></returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static WebqlQuery ParseToAst(string text)
    {
        return WebqlAstBuilder.TranslateQuery(Parse(text));
    }

}

