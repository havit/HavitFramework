using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Havit.Data.EntityFrameworkCore.Patterns.Analyzers.FilteringCollections;

/// <summary>
/// Analyzer that detects usages of <c>FilteringCollection&lt;T&gt;</c> members within expression trees (LINQ to Entities queries).
/// </summary>
/// <remarks>
/// <c>FilteringCollection&lt;T&gt;</c> is an in-memory wrapper over the underlying (mapped) collection, therefore Entity Framework Core
/// cannot translate it to SQL. Such a query either fails at runtime, or - when the member is used within the final projection -
/// silently returns no data at all.
/// <para>
/// Data loaders (<c>IDataLoader</c>, <c>IFluentDataLoader</c>) are excluded: they do support <c>FilteringCollection&lt;T&gt;</c>
/// by substituting the <c>XIncludingDeleted</c> collection. Expression trees consumed outside of a database query
/// (validation rules, mocking setups, ...) have the shape <c>entity =&gt; entity.Collection</c>; that shape is reported only when
/// the expression is passed to an <c>IQueryable</c> method (e.g. <c>Include</c>).
/// </para>
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class FilteringCollectionInExpressionTreeAnalyzer : DiagnosticAnalyzer
{
	/// <inheritdoc/>
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Diagnostics.FilteringCollectionInExpressionTree];

	/// <inheritdoc/>
	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterSyntaxNodeAction(AnalyzeLambda, SyntaxKind.SimpleLambdaExpression, SyntaxKind.ParenthesizedLambdaExpression);
	}

	private void AnalyzeLambda(SyntaxNodeAnalysisContext context)
	{
		var lambda = (LambdaExpressionSyntax)context.Node;

		if (!IsExpressionTree(lambda, context.SemanticModel, context.CancellationToken))
		{
			return;
		}

		// Nested expression tree lambdas are already covered by the analysis of the outermost one.
		if (lambda.Ancestors().OfType<LambdaExpressionSyntax>().Any(ancestor => IsExpressionTree(ancestor, context.SemanticModel, context.CancellationToken)))
		{
			return;
		}

		InvocationExpressionSyntax enclosingInvocation = GetEnclosingInvocation(lambda);
		if ((enclosingInvocation != null) && IsDataLoaderInvocation(enclosingInvocation, context.SemanticModel, context.CancellationToken))
		{
			return;
		}

		SyntaxNode lambdaBody = (lambda.Body is ExpressionSyntax bodyExpression) ? Unparenthesize(bodyExpression) : null;

		foreach (MemberAccessExpressionSyntax memberAccess in lambda.Body.DescendantNodesAndSelf().OfType<MemberAccessExpressionSyntax>())
		{
			ISymbol memberSymbol = context.SemanticModel.GetSymbolInfo(memberAccess, context.CancellationToken).Symbol;
			ITypeSymbol memberType = GetMemberType(memberSymbol);
			if ((memberType == null) || !TryGetFilteringCollectionItemType(memberType, out ITypeSymbol itemType))
			{
				continue;
			}

			// entity => entity.Collection - the shape used by data loaders, validation rules or mocking setups.
			// Within a query (Include, Select, ...) it is still an error, anywhere else it is a legitimate usage.
			if ((memberAccess == lambdaBody)
				&& ((enclosingInvocation == null) || !IsQueryableInvocation(enclosingInvocation, context.SemanticModel, context.CancellationToken)))
			{
				continue;
			}

			context.ReportDiagnostic(Diagnostic.Create(
				Diagnostics.FilteringCollectionInExpressionTree,
				memberAccess.GetLocation(),
				memberSymbol.Name,
				GetSuggestion(memberSymbol, itemType)));
		}
	}

	/// <summary>
	/// Returns true when the lambda is converted to <c>System.Linq.Expressions.Expression</c> (an expression tree), not to a delegate.
	/// </summary>
	private static bool IsExpressionTree(LambdaExpressionSyntax lambda, SemanticModel semanticModel, CancellationToken cancellationToken)
	{
		ITypeSymbol convertedType = semanticModel.GetTypeInfo(lambda, cancellationToken).ConvertedType;

		for (ITypeSymbol type = convertedType; type != null; type = type.BaseType)
		{
			if ((type.Name == FilteringCollectionConstants.ExpressionTypeName)
				&& (type.ContainingNamespace?.ToDisplayString() == FilteringCollectionConstants.ExpressionTypeNamespace))
			{
				return true;
			}
		}

		return false;
	}

	private static ExpressionSyntax Unparenthesize(ExpressionSyntax expression)
	{
		while (expression is ParenthesizedExpressionSyntax parenthesized)
		{
			expression = parenthesized.Expression;
		}

		return expression;
	}

	private static InvocationExpressionSyntax GetEnclosingInvocation(LambdaExpressionSyntax lambda)
	{
		return ((lambda.Parent is ArgumentSyntax argument) && (argument.Parent is ArgumentListSyntax argumentList))
			? argumentList.Parent as InvocationExpressionSyntax
			: null;
	}

	private static bool IsDataLoaderInvocation(InvocationExpressionSyntax invocation, SemanticModel semanticModel, CancellationToken cancellationToken)
	{
		if (semanticModel.GetSymbolInfo(invocation, cancellationToken).Symbol is not IMethodSymbol methodSymbol)
		{
			return false;
		}

		INamedTypeSymbol containingType = (methodSymbol.ReducedFrom ?? methodSymbol).ContainingType;
		if (containingType == null)
		{
			return false;
		}

		return IsDataLoaderType(containingType) || containingType.AllInterfaces.Any(IsDataLoaderType);
	}

	private static bool IsDataLoaderType(INamedTypeSymbol type)
	{
		if (type.ContainingNamespace?.ToDisplayString() != FilteringCollectionConstants.DataLoaderNamespace)
		{
			return false;
		}

		return (type.Name == FilteringCollectionConstants.DataLoaderInterfaceName)
			|| (type.Name == FilteringCollectionConstants.FluentDataLoaderInterfaceName)
			|| (type.Name == FilteringCollectionConstants.FluentDataLoaderExtensionsTypeName);
	}

	private static bool IsQueryableInvocation(InvocationExpressionSyntax invocation, SemanticModel semanticModel, CancellationToken cancellationToken)
	{
		if (semanticModel.GetSymbolInfo(invocation, cancellationToken).Symbol is not IMethodSymbol methodSymbol)
		{
			return false;
		}

		if (IsQueryable(methodSymbol.ReceiverType) || IsQueryable(methodSymbol.ReturnType))
		{
			return true;
		}

		// non-reduced form of an extension method: Queryable.Include(source, navigationPropertyPath)
		return (methodSymbol.Parameters.Length > 0) && IsQueryable(methodSymbol.Parameters[0].Type);
	}

	private static bool IsQueryable(ITypeSymbol type)
	{
		if (type == null)
		{
			return false;
		}

		return IsQueryableInterface(type) || ((type is INamedTypeSymbol namedType) && namedType.AllInterfaces.Any(IsQueryableInterface));
	}

	private static bool IsQueryableInterface(ITypeSymbol type)
	{
		return (type.Name == FilteringCollectionConstants.QueryableInterfaceName)
			&& (type.ContainingNamespace?.ToDisplayString() == FilteringCollectionConstants.QueryableInterfaceNamespace);
	}

	private static ITypeSymbol GetMemberType(ISymbol symbol)
	{
		return symbol switch
		{
			IPropertySymbol property => property.Type,
			IFieldSymbol field => field.Type,
			_ => null
		};
	}

	private static bool TryGetFilteringCollectionItemType(ITypeSymbol type, out ITypeSymbol itemType)
	{
		for (ITypeSymbol currentType = type; currentType != null; currentType = currentType.BaseType)
		{
			if ((currentType.Name == FilteringCollectionConstants.FilteringCollectionTypeName)
				&& (currentType.ContainingNamespace?.ToDisplayString() == FilteringCollectionConstants.FilteringCollectionTypeNamespace)
				&& (currentType is INamedTypeSymbol namedType)
				&& (namedType.TypeArguments.Length == 1))
			{
				itemType = namedType.TypeArguments[0];
				return true;
			}
		}

		itemType = null;
		return false;
	}

	/// <summary>
	/// Returns the name of the mapped counterpart (<c>XIncludingDeleted</c>) when the entity declares one - the very same substitution
	/// the data loader does. Falls back to a generic wording when there is no such member.
	/// </summary>
	private static string GetSuggestion(ISymbol memberSymbol, ITypeSymbol itemType)
	{
		string expectedName = memberSymbol.Name + FilteringCollectionConstants.IncludingDeletedSuffix;

		for (INamedTypeSymbol type = memberSymbol.ContainingType; type != null; type = type.BaseType)
		{
			foreach (ISymbol candidate in type.GetMembers(expectedName))
			{
				if ((GetMemberType(candidate) is INamedTypeSymbol candidateType) && IsEnumerableOf(candidateType, itemType))
				{
					return $"'{expectedName}' with an explicit filter";
				}
			}
		}

		return "the underlying mapped collection with an explicit filter";
	}

	private static bool IsEnumerableOf(INamedTypeSymbol type, ITypeSymbol itemType)
	{
		return type.AllInterfaces.Concat([type]).Any(candidateType =>
			(candidateType.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T)
			&& SymbolEqualityComparer.Default.Equals(candidateType.TypeArguments.FirstOrDefault(), itemType));
	}
}
