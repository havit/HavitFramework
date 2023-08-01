using System;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks;

/// <summary>
/// Reprezentuje změnu v datech.
/// </summary>
internal sealed class EntityChange : Change
{
	/// <summary>
	/// EntityEntry pro implementaci GetCurrentValue a GetOriginalValue.
	/// </summary>
	internal EntityEntry EntityEntry { get; init; }

	/// <inheritdoc />
	public override object GetCurrentValue(IProperty property) => EntityEntry.CurrentValues[property];

	/// <inheritdoc />
	public override object GetOriginalValue(IProperty property) => EntityEntry.OriginalValues[property];
}
