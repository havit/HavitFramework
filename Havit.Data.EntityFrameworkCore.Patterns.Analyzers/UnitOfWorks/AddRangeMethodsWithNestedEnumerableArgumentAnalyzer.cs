using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Havit.Data.EntityFrameworkCore.Patterns.Analyzers.UnitOfWorks;

/// <summary>
/// Analyzer that detects when a nested collection (IEnumerable&lt;IEnumerable&lt;T&gt;&gt;) is passed
/// to UnitOfWork.AddRangeForInsert, AddRangeForInsertAsync, AddRangeForUpdate, or AddRangeForDelete methods.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AddRangeMethodsWithNestedEnumerableArgumentAnalyzer : DiagnosticAnalyzer
{
	private static readonly HashSet<string> s_targetMethodNames = new HashSet<string>(StringComparer.Ordinal)
	{
		UnitOfWorkConstants.AddRangeForInsertMethodName,
		UnitOfWorkConstants.AddRangeForInsertAsyncMethodName,
		UnitOfWorkConstants.AddRangeForUpdateMethodName,
		UnitOfWorkConstants.AddRangeForDeleteMethodName
	};

	private static readonly ImmutableArray<DiagnosticDescriptor> s_supportedDiagnostics = [Diagnostics.UnitOfWorkAddRangeNestedCollection];

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

		// The range overload expects IEnumerable<TEntity>. When a nested collection was passed (IEnumerable<IEnumerable<T>>,
		// IEnumerable<T[]>, T[][], ...), the inferred TEntity is itself an IEnumerable<T> (or an array) => report.
		if (UnitOfWorkAnalyzerHelper.TryGetEnumerableElementType(methodSymbol.TypeArguments[0], enumerableOfTType, out var innerElementType))
		{
			var diagnostic = Diagnostic.Create(
				Diagnostics.UnitOfWorkAddRangeNestedCollection,
				invocation.ArgumentList.Arguments[0].Expression.GetLocation(),
				innerElementType.Name,
				methodName);

			context.ReportDiagnostic(diagnostic);
		}
	}
}
