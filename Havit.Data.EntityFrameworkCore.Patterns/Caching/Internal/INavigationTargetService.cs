namespace Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;

/// <summary>
/// Poskytuje cílový typ kolekcí a dalších navigací dané entity, přesněji typ, který je v takových kolekcích.
/// </summary>
public interface INavigationTargetService
{
	/// <summary>
	/// Poskytuje cílový typ navigace dané entity, přesněji (pokud jde o kolekci) typ, který je v kolekci. Pro vlastnost typu List&lt;Role&gt; vrací typ Role.
	/// </summary>
	NavigationTarget GetNavigationTarget(Type entityType, string propertyName);
}