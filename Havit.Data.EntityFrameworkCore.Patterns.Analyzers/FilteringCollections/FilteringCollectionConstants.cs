namespace Havit.Data.EntityFrameworkCore.Patterns.Analyzers.FilteringCollections;

internal static class FilteringCollectionConstants
{
	internal const string FilteringCollectionTypeName = "FilteringCollection";
	internal const string FilteringCollectionTypeNamespace = "Havit.Model.Collections.Generic";

	internal const string IncludingDeletedSuffix = "IncludingDeleted";

	internal const string ExpressionTypeName = "Expression";
	internal const string ExpressionTypeNamespace = "System.Linq.Expressions";

	internal const string QueryableInterfaceName = "IQueryable";
	internal const string QueryableInterfaceNamespace = "System.Linq";

	internal const string DataLoaderNamespace = "Havit.Data.Patterns.DataLoaders";
	internal const string DataLoaderInterfaceName = "IDataLoader";
	internal const string FluentDataLoaderInterfaceName = "IFluentDataLoader";
	internal const string FluentDataLoaderExtensionsTypeName = "FluentDataLoaderExtensions";
}
