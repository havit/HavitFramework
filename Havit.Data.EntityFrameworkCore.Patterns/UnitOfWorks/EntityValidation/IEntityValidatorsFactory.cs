using System.Collections.Generic;

namespace Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.EntityValidation
{
	/// <summary>
	/// Factory poskytující entity validátory.
	/// </summary>
	public interface IEntityValidatorsFactory
	{
		/// <summary>
		/// Poskytuje entity validátory pro daný typ.
		/// </summary>
		/// <remarks>
		/// Implementace pomocí Castle Windsor nedává případné registrace pro předky entity.
		/// </remarks>
		IEnumerable<IEntityValidator<TEntity>> Create<TEntity>()
			where TEntity : class;

		/// <summary>
		/// Uvolňuje vytvořené validátory.
		/// </summary>
		void Release<TEntity>(IEnumerable<IEntityValidator<TEntity>> validators)
			where TEntity : class;
	}
}
