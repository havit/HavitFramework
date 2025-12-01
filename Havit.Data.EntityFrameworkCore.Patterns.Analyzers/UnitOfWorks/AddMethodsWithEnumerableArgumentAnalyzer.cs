using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Havit.Data.EntityFrameworkCore.Patterns.Analyzers.UnitOfWorks;

/// <summary>
/// Analyzer that detects when IEnumerable&lt;T&gt; is passed to UnitOfWork.AddForInsert, AddForInsertAsync, AddForUpdate, or AddForDelete methods.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AddMethodsWithEnumerableArgumentAnalyzer : DiagnosticAnalyzer
{
	private static readonly string[] s_targetMethodNames =
	[
		UnitOfWorkConstants.AddForInsertMethodName,
		UnitOfWorkConstants.AddForInsertAsyncMethodName,
		UnitOfWorkConstants.AddForUpdateMethodName,
		UnitOfWorkConstants.AddForDeleteMethodName
	];


	/// <inheritdoc/>
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Diagnostics.UnitOfWorkAddIEnumerableArgument];

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

		// Get the first argument
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

		// Check if the argument type is IEnumerable<T>
		if (IsIEnumerable(argumentType, context.Compilation, out ITypeSymbol nestedType))
		{
			string methodWithRangeName = methodName switch
			{
				UnitOfWorkConstants.AddForInsertMethodName => UnitOfWorkConstants.AddRangeForInsertMethodName,
				UnitOfWorkConstants.AddForInsertAsyncMethodName => UnitOfWorkConstants.AddRangeForInsertAsyncMethodName,
				UnitOfWorkConstants.AddForUpdateMethodName => UnitOfWorkConstants.AddRangeForUpdateMethodName,
				UnitOfWorkConstants.AddForDeleteMethodName => UnitOfWorkConstants.AddRangeForDeleteMethodName,
				_ => throw new NotSupportedException(methodName)
			};

			var diagnostic = Diagnostic.Create(
				Diagnostics.UnitOfWorkAddIEnumerableArgument,
				firstArgument.GetLocation(),
				nestedType.Name,
				methodName,
				methodWithRangeName);
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
		if ((containingType.Name == UnitOfWorkConstants.UnitOfWorkInterfaceName)
			&& (containingType.ContainingNamespace?.ToDisplayString() == UnitOfWorkConstants.UnitOfWorkInterfaceNamespace))
		{
			return true;
		}

		// Check if the containing type implements IUnitOfWork
		return containingType.AllInterfaces.Any(i =>
			(i.Name == UnitOfWorkConstants.UnitOfWorkInterfaceName)
			&& (i.ContainingNamespace?.ToDisplayString() == UnitOfWorkConstants.UnitOfWorkInterfaceNamespace));
	}

	private bool IsIEnumerable(ITypeSymbol type, Compilation compilation, out ITypeSymbol nestedType)
	{
		nestedType = null;

		// Get IEnumerable<T> symbol
		var enumerableType = compilation.GetSpecialType(SpecialType.System_Collections_Generic_IEnumerable_T);

		// Check if the type is IEnumerable<T>
		if (type is not INamedTypeSymbol namedType)
		{
			return false;
		}

		// Check if the type itself is IEnumerable<T>
		if (SymbolEqualityComparer.Default.Equals(namedType.OriginalDefinition, enumerableType))
		{
			if (namedType.TypeArguments.Length == 1)
			{
				nestedType = namedType.TypeArguments[0];
			}
			return true;
		}

		// Check if the type implements IEnumerable<T>
		foreach (var interfaceType in namedType.AllInterfaces)
		{
			if (SymbolEqualityComparer.Default.Equals(interfaceType.OriginalDefinition, enumerableType))
			{
				if (interfaceType.TypeArguments.Length == 1)
				{
					nestedType = interfaceType.TypeArguments[0];
				}
				return true;
			}
		}

		return false;
	}
}