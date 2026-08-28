using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Havit.Data.EntityFrameworkCore.Patterns.Analyzers.FilteringCollections;

/// <summary>
/// Analyzer that detects usages of <c>FilteringCollection&lt;T&gt;</c> members within expression trees (LINQ to Entities queries).
/// </summary>
/// <remarks>
/// <c>FilteringCollection&lt;T&gt;</c> is an in-memory wrapper over the underlying (mapped) collection, therefore Entity Framework Core
/// cannot translate it to SQL. Such a query either fails at runtime, or - when the member is used within the final projection -
/// silently returns no data at all.
/// <para>
/// The analysis is operation-based (<see cref="OperationKind.PropertyReference"/>/<see cref="OperationKind.FieldReference"/>),
/// so it covers both method syntax and query syntax - query clauses are already lowered to expression tree lambdas
/// in the operation tree.
/// </para>
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
	private static readonly ImmutableArray<DiagnosticDescriptor> supportedDiagnostics = ImmutableArray.Create(Diagnostics.FilteringCollectionInExpressionTree);

	/// <inheritdoc/>
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => supportedDiagnostics;

	/// <inheritdoc/>
	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterCompilationStartAction(compilationStartContext =>
		{
			KnownTypes knownTypes = KnownTypes.TryResolve(compilationStartContext.Compilation);
			if (knownTypes == null)
			{
				// The compilation does not reference FilteringCollection<T> at all - do not register any action.
				return;
			}

			compilationStartContext.RegisterOperationAction(
				operationContext => AnalyzeMemberReference(operationContext, knownTypes),
				OperationKind.PropertyReference,
				OperationKind.FieldReference);
		});
	}

	private static void AnalyzeMemberReference(OperationAnalysisContext context, KnownTypes knownTypes)
	{
		var memberReference = (IMemberReferenceOperation)context.Operation;

		if (!TryGetFilteringCollectionItemType(memberReference.Type, knownTypes, out ITypeSymbol itemType))
		{
			return;
		}

		// The member reference is relevant only when it sits inside a lambda converted to an expression tree.
		// Each member reference is visited exactly once, so nested (quoted) lambdas need no deduplication;
		// the outermost expression tree lambda determines the consumer (data loader, IQueryable method, ...).
		IAnonymousFunctionOperation outermostExpressionTreeLambda = null;
		for (IOperation current = memberReference.Parent; current != null; current = current.Parent)
		{
			if ((current is IAnonymousFunctionOperation anonymousFunction) && IsConvertedToExpressionTree(anonymousFunction, knownTypes))
			{
				outermostExpressionTreeLambda = anonymousFunction;
			}
		}

		if (outermostExpressionTreeLambda == null)
		{
			return;
		}

		IInvocationOperation enclosingInvocation = GetEnclosingInvocation(outermostExpressionTreeLambda);

		if ((enclosingInvocation != null) && IsDataLoaderInvocation(enclosingInvocation, knownTypes))
		{
			return;
		}

		// entity => entity.Collection - the shape used by data loaders, validation rules or mocking setups.
		// Within a query (Include, Select, ...) it is still an error, anywhere else it is a legitimate usage.
		if (IsWholeLambdaBody(memberReference, outermostExpressionTreeLambda)
			&& ((enclosingInvocation == null) || !IsQueryableInvocation(enclosingInvocation, knownTypes)))
		{
			return;
		}

		context.ReportDiagnostic(Diagnostic.Create(
			Diagnostics.FilteringCollectionInExpressionTree,
			memberReference.Syntax.GetLocation(),
			memberReference.Member.Name,
			GetSuggestion(memberReference.Member, itemType)));
	}

	/// <summary>
	/// Returns true when the anonymous function is converted to <c>System.Linq.Expressions.Expression&lt;TDelegate&gt;</c>
	/// (an expression tree), not to a delegate.
	/// </summary>
	private static bool IsConvertedToExpressionTree(IAnonymousFunctionOperation anonymousFunction, KnownTypes knownTypes)
	{
		// An expression tree conversion is an IConversionOperation to Expression<TDelegate> wrapping the anonymous function
		// (IDelegateCreationOperation is used for conversions to a delegate type only).
		return (anonymousFunction.Parent is IConversionOperation conversion)
			&& (conversion.Type is INamedTypeSymbol convertedType)
			&& SymbolEqualityComparer.Default.Equals(convertedType.OriginalDefinition, knownTypes.ExpressionOfTDelegate);
	}

	/// <summary>
	/// Returns true when the member reference (modulo implicit conversions) forms the whole body of the lambda,
	/// i.e. the lambda has the shape <c>entity =&gt; entity.Collection</c>.
	/// </summary>
	private static bool IsWholeLambdaBody(IMemberReferenceOperation memberReference, IAnonymousFunctionOperation lambda)
	{
		IOperation current = memberReference;
		while ((current.Parent is IConversionOperation conversion) && conversion.IsImplicit)
		{
			current = conversion;
		}

		return (current.Parent is IReturnOperation returnOperation) && (returnOperation.Parent == lambda.Body);
	}

	/// <summary>
	/// Returns the invocation the lambda is passed to as an argument (incl. a <c>params</c> array of expressions), or null.
	/// </summary>
	private static IInvocationOperation GetEnclosingInvocation(IAnonymousFunctionOperation lambda)
	{
		IOperation current = lambda.Parent;
		while (current is IDelegateCreationOperation or IConversionOperation or IArrayInitializerOperation or IArrayCreationOperation)
		{
			current = current.Parent;
		}

		return (current is IArgumentOperation argument)
			? argument.Parent as IInvocationOperation
			: null;
	}

	private static bool IsDataLoaderInvocation(IInvocationOperation invocation, KnownTypes knownTypes)
	{
		IMethodSymbol methodSymbol = invocation.TargetMethod;
		INamedTypeSymbol containingType = (methodSymbol.ReducedFrom ?? methodSymbol).ContainingType;
		if (containingType == null)
		{
			return false;
		}

		return IsDataLoaderType(containingType, knownTypes) || containingType.AllInterfaces.Any(interfaceType => IsDataLoaderType(interfaceType, knownTypes));
	}

	private static bool IsDataLoaderType(INamedTypeSymbol type, KnownTypes knownTypes)
	{
		INamedTypeSymbol typeDefinition = type.OriginalDefinition;

		return SymbolEqualityComparer.Default.Equals(typeDefinition, knownTypes.DataLoader)
			|| SymbolEqualityComparer.Default.Equals(typeDefinition, knownTypes.FluentDataLoader)
			|| SymbolEqualityComparer.Default.Equals(typeDefinition, knownTypes.FluentDataLoaderExtensions);
	}

	private static bool IsQueryableInvocation(IInvocationOperation invocation, KnownTypes knownTypes)
	{
		IMethodSymbol methodSymbol = invocation.TargetMethod;

		if (IsQueryable(methodSymbol.ReceiverType, knownTypes) || IsQueryable(methodSymbol.ReturnType, knownTypes))
		{
			return true;
		}

		// non-reduced form of an extension method: Queryable.Include(source, navigationPropertyPath)
		return (methodSymbol.Parameters.Length > 0) && IsQueryable(methodSymbol.Parameters[0].Type, knownTypes);
	}

	private static bool IsQueryable(ITypeSymbol type, KnownTypes knownTypes)
	{
		if (type == null)
		{
			return false;
		}

		return IsQueryableInterface(type, knownTypes) || ((type is INamedTypeSymbol namedType) && namedType.AllInterfaces.Any(interfaceType => IsQueryableInterface(interfaceType, knownTypes)));
	}

	private static bool IsQueryableInterface(ITypeSymbol type, KnownTypes knownTypes)
	{
		ITypeSymbol typeDefinition = type.OriginalDefinition;

		return SymbolEqualityComparer.Default.Equals(typeDefinition, knownTypes.Queryable)
			|| SymbolEqualityComparer.Default.Equals(typeDefinition, knownTypes.QueryableOfT);
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

	private static bool TryGetFilteringCollectionItemType(ITypeSymbol type, KnownTypes knownTypes, out ITypeSymbol itemType)
	{
		for (ITypeSymbol currentType = type; currentType != null; currentType = currentType.BaseType)
		{
			if ((currentType is INamedTypeSymbol namedType)
				&& SymbolEqualityComparer.Default.Equals(namedType.OriginalDefinition, knownTypes.FilteringCollection))
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

	/// <summary>
	/// Symbols of well-known types, resolved once per compilation. Data loader types may be null (the compilation
	/// does not have to reference Havit.Data.Patterns) - <see cref="SymbolEqualityComparer"/> never matches null.
	/// </summary>
	private sealed class KnownTypes
	{
		public INamedTypeSymbol FilteringCollection { get; private set; }
		public INamedTypeSymbol ExpressionOfTDelegate { get; private set; }
		public INamedTypeSymbol Queryable { get; private set; }
		public INamedTypeSymbol QueryableOfT { get; private set; }
		public INamedTypeSymbol DataLoader { get; private set; }
		public INamedTypeSymbol FluentDataLoader { get; private set; }
		public INamedTypeSymbol FluentDataLoaderExtensions { get; private set; }

		public static KnownTypes TryResolve(Compilation compilation)
		{
			INamedTypeSymbol filteringCollection = compilation.GetTypeByMetadataName(FilteringCollectionConstants.FilteringCollectionMetadataName);
			INamedTypeSymbol expressionOfTDelegate = compilation.GetTypeByMetadataName(FilteringCollectionConstants.ExpressionOfTDelegateMetadataName);

			if ((filteringCollection == null) || (expressionOfTDelegate == null))
			{
				return null;
			}

			return new KnownTypes
			{
				FilteringCollection = filteringCollection,
				ExpressionOfTDelegate = expressionOfTDelegate,
				Queryable = compilation.GetTypeByMetadataName(FilteringCollectionConstants.QueryableMetadataName),
				QueryableOfT = compilation.GetTypeByMetadataName(FilteringCollectionConstants.QueryableOfTMetadataName),
				DataLoader = compilation.GetTypeByMetadataName(FilteringCollectionConstants.DataLoaderMetadataName),
				FluentDataLoader = compilation.GetTypeByMetadataName(FilteringCollectionConstants.FluentDataLoaderMetadataName),
				FluentDataLoaderExtensions = compilation.GetTypeByMetadataName(FilteringCollectionConstants.FluentDataLoaderExtensionsMetadataName),
			};
		}
	}
}
