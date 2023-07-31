namespace Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;

/// <summary>
/// Typ navigace.
/// </summary>
public enum NavigationType
{
	/// <summary>
	/// Klasická reference. Např. Child.Parent, Auto.Barva, ...
	/// Pro účely cachování se neuvažuje (není pro ní podpora, protože není potřeba).
	/// </summary>
	Reference,

	/// <summary>
	/// One-to-one. Zde myšleno "backreference", tedy vazba, kdy mám vlastnost, ale entitu musím hledat podle cizího klíče v jiné entitě.
	/// </summary>
	OneToOne,

	/// <summary>
	/// Vazba, která reprezentuje kolekci OneToMany. Např. Faktura.Items, Master.Children, ...
	/// </summary>
	OneToMany,

	/// <summary>
	/// Vazba, která reprezentuje klasickou kolekci many to many (od EF Core 5). Mezi entitami entity framework core schovává skip navigation. Např. LoginAccount.Roles.
	/// </summary>
	ManyToMany,

	/// <summary>
	/// Vazba, která reprezentuje dekomponovanou vazbu many to many do (jedné či) dvou one to many. Např. LoginAccount.Memberships.
	/// Jde vlastně o speciální případ vazby OneToMany, která má jako cílovou entitu asociační třídu.
	/// </summary>
	ManyToManyDecomposedToOneToMany
}
