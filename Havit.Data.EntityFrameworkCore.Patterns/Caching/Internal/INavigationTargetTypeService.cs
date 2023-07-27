namespace Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;

/// <summary>
/// Poskytuje cílový typ kolekcí a dalších navigací dané entity, přesněji typ, který je v takových kolekcích.
/// </summary>
public interface INavigationTargetTypeService
{
	/// <summary>
	/// Poskytuje cílový typ navigace dané entity, přesněji (pokud jde o kolekci) typ, který je v kolekci. Pro vlastnost typu List&lt;Role&gt; vrací typ Role.
	/// </summary>
	Type GetNavigationTargetType(Type entityType, string propertyName);
}