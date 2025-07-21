using Havit.Diagnostics.Contracts;
using Havit.Linq;

namespace Havit.Data.Patterns.UnitOfWorks;

/// <summary>
/// Extension metody pro <see cref="IUnitOfWork" />.
/// </summary>
public static class UnitOfWorkExt
{
	/// <summary>
	/// Zkrácený zápis pro registraci výstupu metody <see cref="Havit.Linq.CollectionExt.UpdateFrom{TSource, TTarget, TKey}(ICollection{TTarget}, IEnumerable{TSource}, Func{TTarget, TKey}, Func{TSource, TKey}, Func{TSource, TTarget}, Action{TSource, TTarget}, Action{TTarget}, bool)"/>
	/// do <see cref="IUnitOfWork" />.
	/// Zaregistruje:
	/// - <see cref="UpdateFromResult{TTarget}.ItemsAdding" /> pomocí <see cref="IUnitOfWork.AddRangeForInsert{TEntity}(IEnumerable{TEntity})" />,
	/// - <see cref="UpdateFromResult{TTarget}.ItemsUpdating" /> pomocí <see cref="IUnitOfWork.AddRangeForUpdate{TEntity}(IEnumerable{TEntity})" />,
	/// - <see cref="UpdateFromResult{TTarget}.ItemsRemoving" /> pomocí <see cref="IUnitOfWork.AddRangeForDelete{TEntity}(IEnumerable{TEntity})" />.
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

	/// <summary>
	/// Zkrácený zápis pro registraci výstupu metody <see cref="Havit.Linq.CollectionExt.UpdateFrom{TSource, TTarget, TKey}(ICollection{TTarget}, IEnumerable{TSource}, Func{TTarget, TKey}, Func{TSource, TKey}, Func{TSource, TTarget}, Action{TSource, TTarget}, Action{TTarget}, bool)"/>
	/// do <see cref="IUnitOfWork" />.
	/// Zaregistruje:
	/// - <see cref="UpdateFromResult{TTarget}.ItemsAdding" /> pomocí <see cref="IUnitOfWork.AddRangeForInsertAsync{TEntity}(IEnumerable{TEntity}, CancellationToken)" />,
	/// - <see cref="UpdateFromResult{TTarget}.ItemsUpdating" /> pomocí <see cref="IUnitOfWork.AddRangeForUpdate{TEntity}(IEnumerable{TEntity})" />,
	/// - <see cref="UpdateFromResult{TTarget}.ItemsRemoving" /> pomocí <see cref="IUnitOfWork.AddRangeForDelete{TEntity}(IEnumerable{TEntity})" />.
	/// </summary>
	/// <typeparam name="TTarget">typ prvků v cílové kolekci UpdateFrom()</typeparam>
	public static async ValueTask AddUpdateFromResultAsync<TTarget>(this IUnitOfWork unitOfWork, UpdateFromResult<TTarget> updateFromResult, CancellationToken cancellationToken = default)
		where TTarget : class
	{
		Contract.Requires<ArgumentNullException>(unitOfWork != null, nameof(unitOfWork));
		Contract.Requires<ArgumentNullException>(updateFromResult != null, nameof(updateFromResult));

		await unitOfWork.AddRangeForInsertAsync(updateFromResult.ItemsAdding, cancellationToken).ConfigureAwait(false);
		unitOfWork.AddRangeForUpdate(updateFromResult.ItemsUpdating);
		unitOfWork.AddRangeForDelete(updateFromResult.ItemsRemoving);
	}
}
