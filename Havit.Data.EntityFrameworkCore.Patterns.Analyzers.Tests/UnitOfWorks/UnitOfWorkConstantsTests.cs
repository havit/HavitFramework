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

		Assert.AreEqual(UnitOfWorkConstants.AddForInsertMethodName, nameof(IUnitOfWork.AddForInsert));
		Assert.AreEqual(UnitOfWorkConstants.AddForInsertAsyncMethodName, nameof(IUnitOfWork.AddForInsertAsync));
		Assert.AreEqual(UnitOfWorkConstants.AddForUpdateMethodName, nameof(IUnitOfWork.AddForUpdate));
		Assert.AreEqual(UnitOfWorkConstants.AddForDeleteMethodName, nameof(IUnitOfWork.AddForDelete));

		Assert.AreEqual(UnitOfWorkConstants.AddRangeForInsertMethodName, nameof(IUnitOfWork.AddRangeForInsert));
		Assert.AreEqual(UnitOfWorkConstants.AddRangeForInsertAsyncMethodName, nameof(IUnitOfWork.AddRangeForInsertAsync));
		Assert.AreEqual(UnitOfWorkConstants.AddRangeForUpdateMethodName, nameof(IUnitOfWork.AddRangeForUpdate));
		Assert.AreEqual(UnitOfWorkConstants.AddRangeForDeleteMethodName, nameof(IUnitOfWork.AddRangeForDelete));
#pragma warning restore MSTEST0032 // Assertion condition is always true
	}
}
