using Microsoft.CodeAnalysis;

namespace Havit.Data.EntityFrameworkCore.Patterns.Analyzers.UnitOfWorks;

/// <summary>
/// Shared helpers for the UnitOfWork analyzers.
/// </summary>
internal static class UnitOfWorkAnalyzerHelper
{
	/// <summary>
	/// Returns <c>true</c> when <paramref name="methodSymbol"/> is declared on <c>IUnitOfWork</c>
	/// or on a type that implements <c>IUnitOfWork</c>.
	/// </summary>
	internal static bool IsUnitOfWorkMethod(IMethodSymbol methodSymbol)
	{
		var containingType = methodSymbol.ContainingType;
		if (containingType == null)
		{
			return false;
		}

		// The method is declared directly on IUnitOfWork.
		if (IsUnitOfWorkInterface(containingType))
		{
			return true;
		}

		// The method is declared on a type that implements IUnitOfWork.
		// foreach over the (struct-enumerated) ImmutableArray avoids the delegate allocation of LINQ Any.
		foreach (var implementedInterface in containingType.AllInterfaces)
		{
			if (IsUnitOfWorkInterface(implementedInterface))
			{
				return true;
			}
		}

		return false;
	}

	private static bool IsUnitOfWorkInterface(INamedTypeSymbol type)
		=> (type.Name == UnitOfWorkConstants.UnitOfWorkInterfaceName)
			&& (type.ContainingNamespace?.ToDisplayString() == UnitOfWorkConstants.UnitOfWorkInterfaceNamespace);

	/// <summary>
	/// Determines whether <paramref name="type"/> is (or implements) <c>IEnumerable&lt;T&gt;</c> and, if so, returns its element type.
	/// Arrays (e.g. <c>MyEntity[]</c>) are treated as <c>IEnumerable&lt;T&gt;</c> as well.
	/// <c>string</c> is intentionally <em>not</em> treated as a collection - although it implements <c>IEnumerable&lt;char&gt;</c>,
	/// it is never a collection of entities in this context.
	/// </summary>
	internal static bool TryGetEnumerableElementType(ITypeSymbol type, INamedTypeSymbol enumerableOfTType, out ITypeSymbol elementType)
	{
		elementType = null;

		// string is technically IEnumerable<char>, but is never a collection of entities here.
		if (type.SpecialType == SpecialType.System_String)
		{
			return false;
		}

		// Arrays (e.g. MyEntity[]) are IEnumerable<T>.
		if (type is IArrayTypeSymbol arrayType)
		{
			elementType = arrayType.ElementType;
			return true;
		}

		if (type is not INamedTypeSymbol namedType)
		{
			return false;
		}

		// The type itself is IEnumerable<T>.
		if (SymbolEqualityComparer.Default.Equals(namedType.OriginalDefinition, enumerableOfTType)
			&& (namedType.TypeArguments.Length == 1))
		{
			elementType = namedType.TypeArguments[0];
			return true;
		}

		// The type implements IEnumerable<T>.
		foreach (var implementedInterface in namedType.AllInterfaces)
		{
			if (SymbolEqualityComparer.Default.Equals(implementedInterface.OriginalDefinition, enumerableOfTType)
				&& (implementedInterface.TypeArguments.Length == 1))
			{
				elementType = implementedInterface.TypeArguments[0];
				return true;
			}
		}

		return false;
	}
}
