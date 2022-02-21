using System.Collections.Generic;

namespace Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.BeforeCommitProcessors
{
	/// <summary>
	/// Factory poskytující IBeforeCommitProcessors.
	/// </summary>
	/// <remarks>
	/// Revize použití s ohledem na https://github.com/volosoft/castle-windsor-ms-adapter/issues/32:
	/// Implementované služby musí být bezstavové. 
	/// Pokud budou registrované jako transient nebo singleton (což budou), pak se této factory popsaná issue netýká.
	/// </remarks>
	public interface IBeforeCommitProcessorsFactory
	{
		/// <summary>
		/// Poskytuje IBeforeCommitProcessor pro daný typ.
		/// </summary>
		/// <remarks>
		/// Implementace pomocí Castle Windsor nedává případné registrace pro předky entity.
		/// </remarks>
		IEnumerable<IBeforeCommitProcessor<TEntity>> Create<TEntity>()
			where TEntity : class;
	}
}
