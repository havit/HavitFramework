using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Havit.Data.EntityFrameworkCore.Patterns.Analyzers.UnitOfWorks;

/// <summary>
/// Analyzer that detects when a nested collection (IEnumerable&lt;IEnumerable&lt;T&gt;&gt;) is passed
/// to UnitOfWork.AddRangeForInsert, AddRangeForUpdate, or AddRangeForDelete methods.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class UnitOfWorkAddRangeAnalyzer : DiagnosticAnalyzer
{
	private static readonly string[] s_targetMethodNames =
	[
		"AddRangeForInsert",
		"AddRangeForUpdate",
		"AddRangeForDelete",
		"AddRangeForInsertAsync",
		"AddRangeForUpdateAsync",
		"AddRangeForDeleteAsync"
	];

	/// <inheritdoc/>
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Diagnostics.UnitOfWorkAddRangeNestedCollection];

	/// <inheritdoc/>
	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
	}

	private void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
	{
		var invocation = (InvocationExpressionSyntax)context.Node;

		// Get the method being called
		if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess)
		{
			return;
		}

		// Check if the method name matches one of our target methods
		string methodName = memberAccess.Name.Identifier.Text;
		if (!s_targetMethodNames.Contains(methodName))
		{
			return;
		}

		// Get the symbol information for the method
		var symbolInfo = context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken);
		if (symbolInfo.Symbol is not IMethodSymbol methodSymbol)
		{
			return;
		}

		// Check if the method is from IUnitOfWork interface
		if (!IsUnitOfWorkMethod(methodSymbol))
		{
			return;
		}

		// Get the first argument (should be IEnumerable<TEntity>)
		if (invocation.ArgumentList.Arguments.Count == 0)
		{
			return;
		}

		var firstArgument = invocation.ArgumentList.Arguments[0];
		var argumentType = context.SemanticModel.GetTypeInfo(firstArgument.Expression, context.CancellationToken).Type;

		if (argumentType == null)
		{
			return;
		}

		// Check if the argument type is IEnumerable<IEnumerable<T>>
		if (IsNestedEnumerable(argumentType, context.Compilation, out var innerType))
		{
			var diagnostic = Diagnostic.Create(
				Diagnostics.UnitOfWorkAddRangeNestedCollection,
				firstArgument.GetLocation(),
				innerType.Name,
				methodName);

			context.ReportDiagnostic(diagnostic);
		}
	}

	private static bool IsUnitOfWorkMethod(IMethodSymbol methodSymbol)
	{
		// Check if the method is defined in IUnitOfWork or a type that implements it
		var containingType = methodSymbol.ContainingType;
		if (containingType == null)
		{
			return false;
		}

		// Check if it's IUnitOfWork itself
		if (containingType.Name == "IUnitOfWork" &&
			containingType.ContainingNamespace?.ToDisplayString() == "Havit.Data.Patterns.UnitOfWorks")
		{
			return true;
		}

		// Check if the containing type implements IUnitOfWork
		return containingType.AllInterfaces.Any(i =>
			i.Name == "IUnitOfWork" &&
			i.ContainingNamespace?.ToDisplayString() == "Havit.Data.Patterns.UnitOfWorks");
	}

	private bool IsNestedEnumerable(ITypeSymbol type, Compilation compilation, out ITypeSymbol innerType)
	{
		innerType = null;

		// Get IEnumerable<T> symbol
		var enumerableType = compilation.GetSpecialType(SpecialType.System_Collections_Generic_IEnumerable_T);

		// Special handling for array types (e.g., MyEntity[][])
		if (type is IArrayTypeSymbol arrayType)
		{
			var elementType = arrayType.ElementType;

			// Check if the element type is also an array or IEnumerable
			if (elementType is IArrayTypeSymbol innerArrayType)
			{
				innerType = innerArrayType.ElementType;
				return true;
			}

			if (elementType is INamedTypeSymbol elementNamedType)
			{
				var innerEnumerableImplementation = GetEnumerableImplementation(elementNamedType, enumerableType);
				if (innerEnumerableImplementation != null && innerEnumerableImplementation.TypeArguments.Length == 1)
				{
					innerType = innerEnumerableImplementation.TypeArguments[0];
					return true;
				}
			}

			return false;
		}

		// Check if the type implements IEnumerable<T>
		if (type is not INamedTypeSymbol namedType)
		{
			return false;
		}

		// Find if type implements IEnumerable<T>
		var enumerableImplementation = GetEnumerableImplementation(namedType, enumerableType);
		if (enumerableImplementation == null)
		{
			return false;
		}

		// Get the T from IEnumerable<T>
		if (enumerableImplementation.TypeArguments.Length != 1)
		{
			return false;
		}

		var innerElementType = enumerableImplementation.TypeArguments[0];

		// Check if T is an array
		if (innerElementType is IArrayTypeSymbol innerArrayType2)
		{
			innerType = innerArrayType2.ElementType;
			return true;
		}

		// Check if T also implements IEnumerable (making it IEnumerable<IEnumerable<...>>)
		if (innerElementType is INamedTypeSymbol elementNamedType2)
		{
			var innerEnumerableImplementation = GetEnumerableImplementation(elementNamedType2, enumerableType);
			if (innerEnumerableImplementation != null && innerEnumerableImplementation.TypeArguments.Length == 1)
			{
				innerType = innerEnumerableImplementation.TypeArguments[0];
				return true;
			}
		}

		return false;
	}

	private INamedTypeSymbol GetEnumerableImplementation(INamedTypeSymbol type, INamedTypeSymbol enumerableType)
	{
		// Check if the type itself is IEnumerable<T>
		if (SymbolEqualityComparer.Default.Equals(type.OriginalDefinition, enumerableType))
		{
			return type;
		}

		// Check if the type implements IEnumerable<T>
		foreach (var interfaceType in type.AllInterfaces)
		{
			if (SymbolEqualityComparer.Default.Equals(interfaceType.OriginalDefinition, enumerableType))
			{
				return interfaceType;
			}
		}

		return null;
	}
}
