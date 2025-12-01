namespace Havit.Data.EntityFrameworkCore.Patterns.Analyzers.UnitOfWorks;

internal static class UnitOfWorkConstants
{
	internal const string UnitOfWorkInterfaceName = "IUnitOfWork";
	internal const string UnitOfWorkInterfaceNamespace = "Havit.Data.Patterns.UnitOfWorks";

	internal const string AddForInsertMethodName = "AddForInsert";
	internal const string AddForInsertAsyncMethodName = "AddForInsertAsync";
	internal const string AddForUpdateMethodName = "AddForUpdate";
	internal const string AddForDeleteMethodName = "AddForDelete";

	internal const string AddRangeForInsertMethodName = "AddRangeForInsert";
	internal const string AddRangeForInsertAsyncMethodName = "AddRangeForInsertAsync";
	internal const string AddRangeForUpdateMethodName = "AddRangeForUpdate";
	internal const string AddRangeForDeleteMethodName = "AddRangeForDelete";

}
