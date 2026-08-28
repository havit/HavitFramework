namespace Havit.Data.EntityFrameworkCore.Patterns.Analyzers.FilteringCollections;

internal static class FilteringCollectionConstants
{
	internal const string FilteringCollectionMetadataName = "Havit.Model.Collections.Generic.FilteringCollection`1";

	// Konvence pojmenování namapovaného protějšku (X -> XIncludingDeleted) je zduplikovaná vůči runtime substituci
	// v PropertyLoadSequenceResolverIncludingDeletedFilteringCollectionsSubstitution (Havit.Data.EntityFrameworkCore.Patterns).
	// Analyzer projekt na Patterns referencovat nemůže. Při změně konvence je potřeba upravit obě místa.
	internal const string IncludingDeletedSuffix = "IncludingDeleted";

	internal const string ExpressionOfTDelegateMetadataName = "System.Linq.Expressions.Expression`1";

	internal const string QueryableMetadataName = "System.Linq.IQueryable";
	internal const string QueryableOfTMetadataName = "System.Linq.IQueryable`1";

	internal const string DataLoaderMetadataName = "Havit.Data.Patterns.DataLoaders.IDataLoader";
	internal const string FluentDataLoaderMetadataName = "Havit.Data.Patterns.DataLoaders.IFluentDataLoader`1";
	internal const string FluentDataLoaderExtensionsMetadataName = "Havit.Data.Patterns.DataLoaders.FluentDataLoaderExtensions";
}
