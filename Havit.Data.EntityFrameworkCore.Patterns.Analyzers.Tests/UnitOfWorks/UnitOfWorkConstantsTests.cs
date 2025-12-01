using Havit.Data.EntityFrameworkCore.Patterns.Analyzers.UnitOfWorks;
using Havit.Data.Patterns.UnitOfWorks;

namespace Havit.Data.EntityFrameworkCore.Patterns.Analyzers.Tests.UnitOfWorks;

[TestClass]
public class UnitOfWorkConstantsTests
{
	[TestMethod]
	public void UnitOfWorkConstants_Constants()
	{
#pragma warning disable MSTEST0032 // Assertion condition is always true
		Assert.AreEqual(UnitOfWorkConstants.UnitOfWorkInterfaceName, nameof(IUnitOfWork));
		Assert.AreEqual(UnitOfWorkConstants.UnitOfWorkInterfaceNamespace, typeof(IUnitOfWork).Namespace);

		Assert.AreEqual(UnitOfWorkConstants.AddRangeForInsertMethodName, nameof(IUnitOfWork.AddRangeForInsert));
		Assert.AreEqual(UnitOfWorkConstants.AddRangeForUpdateMethodName, nameof(IUnitOfWork.AddRangeForUpdate));
		Assert.AreEqual(UnitOfWorkConstants.AddRangeForDeleteMethodName, nameof(IUnitOfWork.AddRangeForDelete));
		Assert.AreEqual(UnitOfWorkConstants.AddRangeForInsertAsyncMethodName, nameof(IUnitOfWork.AddRangeForInsertAsync));
#pragma warning restore MSTEST0032 // Assertion condition is always true
	}
}
