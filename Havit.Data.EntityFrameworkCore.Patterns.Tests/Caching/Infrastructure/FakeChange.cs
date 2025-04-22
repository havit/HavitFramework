using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Infrastructure;

/// <summary>
/// Implementace Change pro unit testy.
/// </summary>
public class FakeChange : Change
{
	/// <summary>
	/// Hodnoty, které budeme považovat za Original Values entity (hodnoty načtené z databáze, před změnou)
	/// </summary>
	public IDictionary<string, object> OriginalValues { get; set; }

	/// <summary>
	/// Vrátí aktuální hodnotu dané vlastnosti. Získává se z entity samotné, reflexí.
	/// </summary>
	public override object GetCurrentValue(IProperty property)
	{
		if (Entity is IDictionary<string, object> entityDictionary)
		{
			return entityDictionary[property.Name];
		}

		return Entity.GetType().GetProperty(property.Name).GetValue(Entity, null);
	}

	/// <summary>
	/// Vrátí původní hodnotu dané vlastnosti. Získává se z předané dictionary původních hodnot.
	/// </summary>
	public override object GetOriginalValue(IProperty property)
	{
		if (OriginalValues == null)
		{
			throw new InvalidOperationException("Dictionary with original values was not initialized.");
		}
		return OriginalValues[property.Name];
	}
}
