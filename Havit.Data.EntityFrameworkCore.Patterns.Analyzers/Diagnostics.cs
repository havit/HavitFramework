using Havit.Diagnostics.Common;
using Microsoft.CodeAnalysis;

namespace Havit.Data.EntityFrameworkCore.Patterns.Analyzers;

/// <summary>
/// Provides diagnostics for detecting improper usage of AddRangeForInsert, AddRangeForUpdate,
/// and AddRangeForDelete methods in the UnitOfWork pattern. Specifically, it identifies instances
/// where nested collections (IEnumerable) are mistakenly passed to these methods
/// instead of a flat IEnumerable of T.
/// </summary>
public static class Diagnostics
{
	/// <summary>
	/// Represents a diagnostic descriptor that identifies and reports cases where a nested collection
	/// is passed to methods such as AddRangeForInsert, AddRangeForUpdate,
	/// or AddRangeForDelete, which expect a flat IEnumerable of T.
	/// </summary>
	/// <remarks>
	/// This diagnostic helps ensure correct data structure usage when invoking methods intended to handle
	/// a single-level collection of entities by warning against passing multi-level (nested) collections,
	/// which could lead to unintended behavior or runtime errors.
	/// </remarks>
	/// <example>
	/// When the diagnostic detects a nested collection being passed, it produces a warning with a detailed
	/// message format that specifies the type of the entity and the method involved.
	/// </example>
	public static readonly DiagnosticDescriptor UnitOfWorkAddRangeNestedCollection = new DiagnosticDescriptor(
		id: DiagnosticIdentifiers.UnitOfWorkAddRangeNestedCollectionId,
		title: "Nested collection passed to AddRangeFor* method",
		messageFormat: "Passing a nested collection (IEnumerable<IEnumerable<{0}>>) to '{1}' is likely incorrect. The method expects IEnumerable<{0}>, not a collection of collections.",
		category: "Usage",
		defaultSeverity: DiagnosticSeverity.Warning,
		isEnabledByDefault: true,
		description: "Detects when IEnumerable<IEnumerable<T>> is passed to AddRangeForInsert, AddRangeForUpdate, or AddRangeForDelete methods instead of IEnumerable<T>."
	);
}
