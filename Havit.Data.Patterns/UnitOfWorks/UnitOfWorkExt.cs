using System;
using Havit.Diagnostics.Contracts;
using Havit.Linq;

namespace Havit.Data.Patterns.UnitOfWorks;

/// <summary>
/// Extension metody pro <c>IUnitOfWork</c>.
/// </summary>
public static class UnitOfWorkExt
{
	/// <summary>
	/// Zkrácený zápis pro přesypání výstupu metody UpdateFrom() do UoW.
	/// Přeleje ItemsAdding do AddRangeForInsert(), ItemsUpdated do AddRangeForUpdate() a ItemsRemoved do AddRangeForDelete().
	/// </summary>
	/// <typeparam name="TTarget">typ prvků v cílové kolekci UpdateFrom()</typeparam>
	public static void AddUpdateFromResult<TTarget>(this IUnitOfWork unitOfWork, UpdateFromResult<TTarget> updateFromResult)
		where TTarget : class
	{
		Contract.Requires<ArgumentNullException>(unitOfWork != null, nameof(unitOfWork));
		Contract.Requires<ArgumentNullException>(updateFromResult != null, nameof(updateFromResult));

		unitOfWork.AddRangeForInsert(updateFromResult.ItemsAdding);
		unitOfWork.AddRangeForUpdate(updateFromResult.ItemsUpdating);
		unitOfWork.AddRangeForDelete(updateFromResult.ItemsRemoving);
	}
}
