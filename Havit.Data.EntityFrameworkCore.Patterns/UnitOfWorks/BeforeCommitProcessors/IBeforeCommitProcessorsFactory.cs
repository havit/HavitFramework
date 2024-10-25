using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.BeforeCommitProcessors.Internal;

namespace Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.BeforeCommitProcessors;

/// <summary>
/// Factory poskytující IBeforeCommitProcessors.
/// </summary>
public interface IBeforeCommitProcessorsFactory
{
	/// <summary>
	/// Poskytuje IBeforeCommitProcessor pro daný typ.
	/// V případě vícero registrací vrací nejprve obecnější typy, později konkrétnější typy.
	/// </summary>
	IEnumerable<IBeforeCommitProcessorInternal> Create(Type entityType);
}
