using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Services;

namespace Havit.Data.Entity.Patterns.UnitOfWorks.BeforeCommitProcessors;

/// <summary>
/// Factory poskytující IBeforeCommitProcessors.
/// </summary>
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

	/// <summary>
	/// Uvolňuje vytvořené procesory.
	/// </summary>
	void Release<TEntity>(IEnumerable<IBeforeCommitProcessor<TEntity>> processors)
		where TEntity : class;
}
