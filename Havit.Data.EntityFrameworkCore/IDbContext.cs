using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.Entity
{
	/// <summary>
	/// Interface DbContextu.
	/// Poskytuje služby DbContextu vyšším vrstvám.
	/// Umožňuje např. v testech podstrčit jinou instanci DbContextu.
	/// </summary>
	public interface IDbContext
	{
		/// <summary>
		/// Creates a DbSet&lt;TEntity&gt; that can be used to query and save instances of TEntity.
		/// </summary>
		/// <remarks>
		/// Pro snažší možnost mockování konzumentů DbSetu je vytvořena abstrakce do interface IDbSet&lt;TEntity&gt;.
		/// </remarks>
		IDbSet<TEntity> Set<TEntity>()
			where TEntity : class;

		/// <summary>
		/// The metadata about the shape of entities, the relationships between them, and how they map to the database.
		/// </summary>
		/// <remarks>
		/// Pro účely zjednodušení předáváme celý model. Pro účely lepšího mockování jako závislosti můžeme nahradit jednoúčelovými metodami.
		/// </remarks>
		IModel Model { get; }

		/// <summary>
		/// Uloží změny.
		/// </summary>
		void SaveChanges();

		/// <summary>
		/// Uloží změny.
		/// </summary>
		Task SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Registruje akci k provedení po save changes. Akce je provedena metodou AfterSaveChanges.
		/// </summary>
		void RegisterAfterSaveChangesAction(Action action);

		/// <summary>
		/// Vrátí aktuální stav entity.
		/// </summary>
		EntityState GetEntityState<TEntity>(TEntity entity)
			where TEntity : class;

		/// <summary>
		/// Vrátí objekty v daných stavech.
		/// </summary>
		object[] GetObjectsInState(EntityState state, bool suppressDetectChanges);

		/// <summary>
		/// Vrátí objekty v daných stavech.
		/// </summary>
		object[] GetObjectsInStates(EntityState[] states, bool suppressDetectChanges);

		/// <summary>
		/// Vrací true, pokud je EF považuje referenci za načtenou.
		/// </summary>
		bool IsEntityReferenceLoaded<TEntity>(TEntity entity, string propertyName)
			where TEntity : class;
	
		/// <summary>
		/// Vrací true, pokud je EF považuje kolekci za načtenou.
		/// </summary>
		bool IsEntityCollectionLoaded<TEntity>(TEntity entity, string propertyName)
			where TEntity : class;

		//void SetEntityCollectionLoaded<TEntity>(TEntity entity, string propertyName, bool isLoaded)
		//	where TEntity : class;
	}
}