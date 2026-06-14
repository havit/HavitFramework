using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Havit.Data.EntityFrameworkCore.Patterns.Analyzers.UnitOfWorks;

/// <summary>
/// Analyzer that detects when a collection (IEnumerable&lt;T&gt; or an array) is passed to
/// UnitOfWork.AddForInsert, AddForInsertAsync, AddForUpdate, or AddForDelete methods, which expect a single entity.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AddMethodsWithEnumerableArgumentAnalyzer : DiagnosticAnalyzer
{
	private static readonly HashSet<string> s_targetMethodNames = new HashSet<string>(StringComparer.Ordinal)
	{
		UnitOfWorkConstants.AddForInsertMethodName,
		UnitOfWorkConstants.AddForInsertAsyncMethodName,
		UnitOfWorkConstants.AddForUpdateMethodName,
		UnitOfWorkConstants.AddForDeleteMethodName
	};

	private static readonly ImmutableArray<DiagnosticDescriptor> s_supportedDiagnostics = [Diagnostics.UnitOfWorkAddIEnumerableArgument];

	/// <inheritdoc/>
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => s_supportedDiagnostics;

	/// <inheritdoc/>
	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterCompilationStartAction(compilationStartContext =>
		{
			// Resolve IEnumerable<T> once per compilation instead of on every analyzed node.
			var enumerableOfTType = compilationStartContext.Compilation.GetSpecialType(SpecialType.System_Collections_Generic_IEnumerable_T);

			compilationStartContext.RegisterSyntaxNodeAction(
				nodeContext => AnalyzeInvocation(nodeContext, enumerableOfTType),
				SyntaxKind.InvocationExpression);
		});
	}

	private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context, INamedTypeSymbol enumerableOfTType)
	{
		var invocation = (InvocationExpressionSyntax)context.Node;

		// Cheap syntactic gate first: a member access to one of our target method names.
		if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess)
		{
			return;
		}

		string methodName = memberAccess.Name.Identifier.Text;
		if (!s_targetMethodNames.Contains(methodName))
		{
			return;
		}

		// Only now reach for the (more expensive) semantic model.
		var symbolInfo = context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken);
		if (symbolInfo.Symbol is not IMethodSymbol methodSymbol)
		{
			return;
		}

		// All target methods are generic with a single type parameter (TEntity).
		if (methodSymbol.TypeArguments.Length != 1)
		{
			return;
		}

		if (!UnitOfWorkAnalyzerHelper.IsUnitOfWorkMethod(methodSymbol))
		{
			return;
		}

		if (invocation.ArgumentList.Arguments.Count == 0)
		{
			return;
		}

		// The single-entity overload was called with a collection when the inferred TEntity is itself an IEnumerable<T> (or an array).
		// Using the inferred type argument (instead of the syntactic argument type) also covers arrays uniformly.
		if (UnitOfWorkAnalyzerHelper.TryGetEnumerableElementType(methodSymbol.TypeArguments[0], enumerableOfTType, out var elementType))
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
				invocation.ArgumentList.Arguments[0].Expression.GetLocation(),
				elementType.Name,
				methodName,
				methodWithRangeName);
			context.ReportDiagnostic(diagnostic);
		}
	}
}
