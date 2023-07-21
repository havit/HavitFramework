using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks;

/// <summary>
/// Změny objektů v UnitOfWork.
/// </summary>
public class Changes : IEnumerable<Change>
{
	private List<Change> changes;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public Changes(IEnumerable<Change> changes)
	{
		this.changes = changes.ToList();
	}

	/// <summary>
	/// Registrované objekty pro Insert. Pro zpětnou kompatibilitu.
	/// </summary>
	[Obsolete("Changes samotné je nyní IEnumerable<Change>, obsahuje nejen entity, ale i informace k nim." )]
	public object[] Inserts => this.Where(item => item.ChangeType == ChangeType.Insert).Select(item => item.Entity).ToArray();

	/// <summary>
	/// Registrované objekty pro Update. Pro zpětnou kompatibilitu.
	/// </summary>
	[Obsolete("Changes samotné je nyní IEnumerable<Change>, obsahuje nejen entity, ale i informace k nim.")]
	public object[] Updates => this.Where(item => item.ChangeType == ChangeType.Update).Select(item => item.Entity).ToArray();

	/// <summary>
	/// Registrované objekty pro Delete. Pro zpětnou kompatibilitu.
	/// </summary>
	[Obsolete("Changes samotné je nyní IEnumerable<Change>, obsahuje nejen entity, ale i informace k nim.")]
	public object[] Deletes => this.Where(item => item.ChangeType == ChangeType.Delete).Select(item => item.Entity).ToArray();

	/// <summary>
	/// Vrátí všechny změněné objekty (bez ohledu na způsob změny).
	/// Pro zpětnou kompatibilitu.
	/// </summary>
	[Obsolete("Changes samotné je nyní IEnumerable<Change>, obsahuje nejen entity, ale i informace k nim.")]
	public object[] GetAllChanges()
	{
		return this.Select(item => item.Entity).ToArray();
	}

	#region IEnumerable<Change> implementation
	IEnumerator<Change> IEnumerable<Change>.GetEnumerator() => changes.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => changes.GetEnumerator();
	#endregion
}