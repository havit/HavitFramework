using Havit.Diagnostics.Common;
using Microsoft.CodeAnalysis;

namespace Havit.Data.EntityFrameworkCore.Patterns.Analyzers;

/// <summary>
/// Provides diagnostics for detecting improper usage methods in the UnitOfWork pattern.
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
	public static readonly DiagnosticDescriptor UnitOfWorkAddRangeNestedCollection = new DiagnosticDescriptor(
		id: DiagnosticIdentifiers.UnitOfWorkAddRangeNestedCollectionId,
		title: "Nested collection passed to AddRangeFor* method",
		messageFormat: "Passing a nested collection (IEnumerable<IEnumerable<{0}>>) to '{1}' is likely incorrect. The method expects IEnumerable<{0}>, not a collection of collections.",
		category: "Usage",
		defaultSeverity: DiagnosticSeverity.Warning,
		isEnabledByDefault: true,
		description: "Detects when IEnumerable<IEnumerable<T>> is passed to AddRangeForInsert, AddRangeForInsertAsync, AddRangeForUpdate, or AddRangeForDelete methods instead of IEnumerable<T>."
	);

	/// <summary>
	/// Represents a diagnostic descriptor that identifies and reports cases where IEnumerable&lt;T&gt; 
	/// is passed to methods such as AddForInsert, AddForInsertAsync, AddForUpdate, or AddForDelete
	/// which expect a single entity instance of type T.
	/// </summary>
	/// <remarks>
	/// This diagnostic helps prevent incorrect usage where a collection is passed to methods designed 
	/// to handle individual entities. Use AddRangeForInsert/AddRangeForInsertAsync/AddRangeForUpdate/AddRangeForDelete methods 
	/// for collection operations instead.
	/// </remarks>
	public static readonly DiagnosticDescriptor UnitOfWorkAddIEnumerableArgument = new DiagnosticDescriptor(
		id: DiagnosticIdentifiers.UnitOfWorkAddIEnumerableArgumentId,
		title: "IEnumerable passed to AddFor* method",
		messageFormat: "Passing a collection (IEnumerable<{0}>) to '{1}' is likely incorrect. The method expects a single entity instance of {0}, not a collection. Use {2} method for collections.",
		category: "Usage",
		defaultSeverity: DiagnosticSeverity.Warning,
		isEnabledByDefault: true,
		description: "Detects when IEnumerable<T> is passed to AddForInsert, AddForInsertAsync, AddForUpdate, or AddForDelete methods instead of a single entity."
	);

	/// <summary>
	/// Represents a diagnostic descriptor that identifies and reports cases where a member of type
	/// <c>FilteringCollection&lt;T&gt;</c> is used within an expression tree which is translated to a database query.
	/// </summary>
	/// <remarks>
	/// <c>FilteringCollection&lt;T&gt;</c> is an in-memory wrapper over the underlying (mapped) collection, it is not
	/// a mapped navigation property. Entity Framework Core is therefore not able to translate it to SQL: the query either
	/// fails at runtime, or - when the member is used within the final projection - silently returns no data at all.
	/// </remarks>
	public static readonly DiagnosticDescriptor FilteringCollectionInExpressionTree = new DiagnosticDescriptor(
		id: DiagnosticIdentifiers.FilteringCollectionInExpressionTreeId,
		title: "FilteringCollection used within a database query",
		messageFormat: "'{0}' is a FilteringCollection which cannot be translated to SQL - the query fails at runtime or silently returns no data. Use {1} instead.",
		category: "Usage",
		defaultSeverity: DiagnosticSeverity.Warning,
		isEnabledByDefault: true,
		description: "Detects when a FilteringCollection<T> member is used within an expression tree (LINQ to Entities query). FilteringCollection<T> is an in-memory wrapper which Entity Framework Core cannot translate to SQL."
	);
}
