using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore
{
	/// <summary>
	/// Interface DbContextu.
	/// Poskytuje služby DbContextu vyšším vrstvám.
	/// Umožňuje např. v testech podstrčit jinou instanci DbContextu.
	/// </summary>
	public interface IDbContext : IDisposable
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
		/// Provides access to database related information and operations for this context.
		/// </summary>
		/// <remarks>
		/// Zveřejněno (bez wrapperu) pro možnost použít EF Core Migrations (a extension metodu Migrate()).
		/// </remarks>
		DatabaseFacade Database { get; }

		/// <summary>
		/// Provides access to information and operations for entity instances this context is tracking.
		/// </summary>
		/// <remarks>
		/// Pro zjednodušení předáváme celý objekt bez wrapperu.
		/// </remarks>
		ChangeTracker ChangeTracker { get; }

		/// <summary>
		/// Uloží změny.
		/// </summary>
		void SaveChanges();

		/// <summary>
		/// Uloží změny.
		/// </summary>
		Task SaveChangesAsync(CancellationToken cancellationToken = default);

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
		/// Vrací true, pokud je EF považuje vlastnost za načtenou.
		/// </summary>
		bool IsNavigationLoaded<TEntity>(TEntity entity, string propertyName)
			where TEntity : class;
	
		/// <summary>
		/// Nastaví navigation property (kolekce, reference) jako načtenou.
		/// </summary>
		void MarkNavigationAsLoaded<TEntity>(TEntity entity, string propertyName)
			where TEntity : class;

        /// <summary>
        /// Vrací EntityEntry pro danou entitu bez provedení detekce změn change trackerem.
        /// </summary>
        EntityEntry GetEntry(object entity, bool suppressDetectChanges);
    }
}