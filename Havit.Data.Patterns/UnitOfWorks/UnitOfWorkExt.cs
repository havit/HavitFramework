using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Diagnostics.Contracts;
using Havit.Linq;

namespace Havit.Data.Patterns.UnitOfWorks
{
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
			Contract.Requires<ArgumentNullException>(unitOfWork != null);
			Contract.Requires<ArgumentNullException>(updateFromResult != null);

			unitOfWork.AddRangeForInsert(updateFromResult.ItemsAdding);
			unitOfWork.AddRangeForUpdate(updateFromResult.ItemsUpdating);
			unitOfWork.AddRangeForDelete(updateFromResult.ItemsRemoving);
		}

		/// <summary>
		/// Zkrácený zápis pro přesypání výstupu metody UpdateFrom() do UoW.
		/// Přeleje ItemsAdding do AddRangeForInsert(), ItemsUpdated do AddRangeForUpdate() a ItemsRemoved do AddRangeForDelete().
		/// </summary>
		/// <typeparam name="TTarget">typ prvků v cílové kolekci UpdateFrom()</typeparam>
		public static void AddUpdateFromResult<TTarget>(this IUnitOfWorkAsync unitOfWork, UpdateFromResult<TTarget> updateFromResult)
			where TTarget : class
		{
			Contract.Requires<ArgumentNullException>(unitOfWork != null);
			Contract.Requires<ArgumentNullException>(updateFromResult != null);

			unitOfWork.AddRangeForInsert(updateFromResult.ItemsAdding);
			unitOfWork.AddRangeForUpdate(updateFromResult.ItemsUpdating);
			unitOfWork.AddRangeForDelete(updateFromResult.ItemsRemoving);
		}
	}
}
