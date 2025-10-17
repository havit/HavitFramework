using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Havit.Data.EntityFrameworkCore;

/// <summary>
/// Abstrakce DbSet a jeho služeb pro možnost snadného mockování v závislostech.
/// </summary>
public interface IDbSet<TEntity>
	where TEntity : class
{
	/// <summary>
	/// Vrátí data DbSetu jako <see cref="IQueryable{TEntity}"/>.
	/// <see cref="DbSet{TEntity}"/> je <see cref="IQueryable{TEntity}"/> sám o sobě. Pro možnost snadné implementace a mockování získáme <see cref="IQueryable{TEntity}"/> touto metodou.
	/// </summary>
	/// <param name="queryTag">
	/// Pokud je zadán, automaticky se použije jako tag k SQL dotazu (<see cref="IQueryable{TEntity}"/>.TagWith(...)).
	/// </param>
	IQueryable<TEntity> AsQueryable(string queryTag);

	/// <summary>
	/// Vyhledá entitu v načtených (trackovaných objektech). Pokud objekt není nalezen, vrací null.
	/// </summary>
	TEntity FindTracked(params object[] keyValues);

	/// <summary>
	///     Begins tracking the given entity, and any other reachable entities that are
	///     not already being tracked, in the <see cref="EntityState.Added" /> state such that they will
	///     be inserted into the database when <see cref="Microsoft.EntityFrameworkCore.DbContext.SaveChanges()" /> is called.
	/// </summary>
	/// <remarks>
	///     <para>
	///         Use <see cref="EntityEntry.State" /> to set the state of only a single entity.
	///     </para>
	///     <para>
	///         See <see href="https://aka.ms/efcore-docs-change-tracking">EF Core change tracking</see> for more information and examples.
	///     </para>
	/// </remarks>
	void Add(TEntity entity);

	/// <summary>
	///     Begins tracking the given entity, and any other reachable entities that are
	///     not already being tracked, in the <see cref="EntityState.Added" /> state such that they will
	///     be inserted into the database when <see cref="Microsoft.EntityFrameworkCore.DbContext.SaveChanges()" /> is called.
	/// </summary>
	/// <remarks>
	///     <para>
	///         This method is async only to allow special value generators, such as the one used by
	///         'Microsoft.EntityFrameworkCore.Metadata.SqlServerValueGenerationStrategy.SequenceHiLo',
	///         to access the database asynchronously. For all other cases the non async method should be used.
	///     </para>
	///     <para>
	///         Use <see cref="EntityEntry.State" /> to set the state of only a single entity.
	///     </para>
	///     <para>
	///         See <see href="https://aka.ms/efcore-docs-change-tracking">EF Core change tracking</see> for more information and examples.
	///     </para>
	/// </remarks>
	ValueTask AddAsync(TEntity entity, CancellationToken cancellationToken = default);

	/// <summary>
	///     Begins tracking the given entities, and any other reachable entities that are
	///     not already being tracked, in the <see cref="EntityState.Added" /> state such that they will
	///     be inserted into the database when <see cref="Microsoft.EntityFrameworkCore.DbContext.SaveChanges()" /> is called.
	/// </summary>
	/// <remarks>
	///     See <see href="https://aka.ms/efcore-docs-change-tracking">EF Core change tracking</see>
	///     and <see href="https://aka.ms/efcore-docs-attach-range">Using AddRange, UpdateRange, AttachRange, and RemoveRange</see>
	///     for more information and examples.
	/// </remarks>
	/// <param name="entities">The entities to add.</param>
	void AddRange(IEnumerable<TEntity> entities);

	/// <summary>
	///     Begins tracking the given entities, and any other reachable entities that are
	///     not already being tracked, in the <see cref="EntityState.Added" /> state such that they will
	///     be inserted into the database when <see cref="Microsoft.EntityFrameworkCore.DbContext.SaveChanges()" /> is called.
	/// </summary>
	/// <remarks>
	///     <para>
	///         This method is async only to allow special value generators, such as the one used by
	///         'Microsoft.EntityFrameworkCore.Metadata.SqlServerValueGenerationStrategy.SequenceHiLo',
	///         to access the database asynchronously. For all other cases the non async method should be used.
	///     </para>
	///     <para>
	///         See <see href="https://aka.ms/efcore-docs-change-tracking">EF Core change tracking</see>
	///         and <see href="https://aka.ms/efcore-docs-attach-range">Using AddRange, UpdateRange, AttachRange, and RemoveRange</see>
	///         for more information and examples.
	///     </para>
	/// </remarks>
	ValueTask AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

	/// <summary>
	///     Begins tracking the given entity and entries reachable from the given entity using
	///     the <see cref="EntityState.Modified" /> state by default, but see below for cases
	///     when a different state will be used.
	/// </summary>
	/// <remarks>
	///     <para>
	///         Generally, no database interaction will be performed until <see cref="Microsoft.EntityFrameworkCore.DbContext.SaveChanges()" /> is called.
	///     </para>
	///     <para>
	///         A recursive search of the navigation properties will be performed to find reachable entities
	///         that are not already being tracked by the context. All entities found will be tracked
	///         by the context.
	///     </para>
	///     <para>
	///         For entity types with generated keys if an entity has its primary key value set
	///         then it will be tracked in the <see cref="EntityState.Modified" /> state. If the primary key
	///         value is not set then it will be tracked in the <see cref="EntityState.Added" /> state.
	///         This helps ensure new entities will be inserted, while existing entities will be updated.
	///         An entity is considered to have its primary key value set if the primary key property is set
	///         to anything other than the CLR default for the property type.
	///     </para>
	///     <para>
	///         For entity types without generated keys, the state set is always <see cref="EntityState.Modified" />.
	///     </para>
	///     <para>
	///         Use <see cref="EntityEntry.State" /> to set the state of only a single entity.
	///     </para>
	///     <para>
	///         See <see href="https://aka.ms/efcore-docs-change-tracking">EF Core change tracking</see> for more information and examples.
	///     </para>
	/// </remarks>
	void Update(TEntity entity);

	/// <summary>
	///     Begins tracking the given entities and entries reachable from the given entities using
	///     the <see cref="EntityState.Modified" /> state by default, but see below for cases
	///     when a different state will be used.
	/// </summary>
	/// <remarks>
	///     <para>
	///         Generally, no database interaction will be performed until <see cref="Microsoft.EntityFrameworkCore.DbContext.SaveChanges()" /> is called.
	///     </para>
	///     <para>
	///         A recursive search of the navigation properties will be performed to find reachable entities
	///         that are not already being tracked by the context. All entities found will be tracked
	///         by the context.
	///     </para>
	///     <para>
	///         For entity types with generated keys if an entity has its primary key value set
	///         then it will be tracked in the <see cref="EntityState.Modified" /> state. If the primary key
	///         value is not set then it will be tracked in the <see cref="EntityState.Added" /> state.
	///         This helps ensure new entities will be inserted, while existing entities will be updated.
	///         An entity is considered to have its primary key value set if the primary key property is set
	///         to anything other than the CLR default for the property type.
	///     </para>
	///     <para>
	///         For entity types without generated keys, the state set is always <see cref="EntityState.Modified" />.
	///     </para>
	///     <para>
	///         Use <see cref="EntityEntry.State" /> to set the state of only a single entity.
	///     </para>
	///     <para>
	///         See <see href="https://aka.ms/efcore-docs-change-tracking">EF Core change tracking</see>
	///         and <see href="https://aka.ms/efcore-docs-attach-range">Using AddRange, UpdateRange, AttachRange, and RemoveRange</see>
	///         for more information and examples.
	///     </para>
	/// </remarks>
	/// <param name="entities">The entities to update.</param>	
	void UpdateRange(IEnumerable<TEntity> entities);

	/// <summary>
	///     Begins tracking the given entity in the <see cref="EntityState.Deleted" /> state such that it will
	///     be removed from the database when <see cref="Microsoft.EntityFrameworkCore.DbContext.SaveChanges()" /> is called.
	/// </summary>
	/// <remarks>
	///     <para>
	///         If the entity is already tracked in the <see cref="EntityState.Added" /> state then the context will
	///         stop tracking the entity (rather than marking it as <see cref="EntityState.Deleted" />) since the
	///         entity was previously added to the context and does not exist in the database.
	///     </para>
	///     <para>
	///         Any other reachable entities that are not already being tracked will be tracked in the same way that
	///         they would be if <see cref="Attach(TEntity)" /> was called before calling this method.
	///         This allows any cascading actions to be applied when <see cref="Microsoft.EntityFrameworkCore.DbContext.SaveChanges()" /> is called.
	///     </para>
	///     <para>
	///         Use <see cref="EntityEntry.State" /> to set the state of only a single entity.
	///     </para>
	///     <para>
	///         See <see href="https://aka.ms/efcore-docs-change-tracking">EF Core change tracking</see> for more information and examples.
	///     </para>
	/// </remarks>
	void Remove(TEntity entity);

	/// <summary>
	///     Begins tracking the given entities in the <see cref="EntityState.Deleted" /> state such that they will
	///     be removed from the database when <see cref="Microsoft.EntityFrameworkCore.DbContext.SaveChanges()" /> is called.
	/// </summary>
	/// <remarks>
	///     <para>
	///         If any of the entities are already tracked in the <see cref="EntityState.Added" /> state then the context will
	///         stop tracking those entities (rather than marking them as <see cref="EntityState.Deleted" />) since those
	///         entities were previously added to the context and do not exist in the database.
	///     </para>
	///     <para>
	///         Any other reachable entities that are not already being tracked will be tracked in the same way that
	///         they would be if <see cref="AttachRange(IEnumerable{TEntity})" /> was called before calling this method.
	///         This allows any cascading actions to be applied when <see cref="Microsoft.EntityFrameworkCore.DbContext.SaveChanges()" /> is called.
	///     </para>
	///     <para>
	///         See <see href="https://aka.ms/efcore-docs-change-tracking">EF Core change tracking</see>
	///         and <see href="https://aka.ms/efcore-docs-attach-range">Using AddRange, UpdateRange, AttachRange, and RemoveRange</see>
	///         for more information and examples.
	///     </para>
	/// </remarks>
	/// <param name="entities">The entities to remove.</param>
	void RemoveRange(IEnumerable<TEntity> entities);

	/// <summary>
	///     Begins tracking the given entity and entries reachable from the given entity using
	///     the <see cref="EntityState.Unchanged" /> state by default, but see below for cases
	///     when a different state will be used.
	/// </summary>
	/// <remarks>
	///     <para>
	///         Generally, no database interaction will be performed until <see cref="Microsoft.EntityFrameworkCore.DbContext.SaveChanges()" /> is called.
	///     </para>
	///     <para>
	///         A recursive search of the navigation properties will be performed to find reachable entities
	///         that are not already being tracked by the context. All entities found will be tracked
	///         by the context.
	///     </para>
	///     <para>
	///         For entity types with generated keys if an entity has its primary key value set
	///         then it will be tracked in the <see cref="EntityState.Unchanged" /> state. If the primary key
	///         value is not set then it will be tracked in the <see cref="EntityState.Added" /> state.
	///         This helps ensure only new entities will be inserted.
	///         An entity is considered to have its primary key value set if the primary key property is set
	///         to anything other than the CLR default for the property type.
	///     </para>
	///     <para>
	///         For entity types without generated keys, the state set is always <see cref="EntityState.Unchanged" />.
	///     </para>
	///     <para>
	///         Use <see cref="EntityEntry.State" /> to set the state of only a single entity.
	///     </para>
	///     <para>
	///         See <see href="https://aka.ms/efcore-docs-change-tracking">EF Core change tracking</see> for more information and examples.
	///     </para>
	/// </remarks>
	void Attach(TEntity entity);

	/// <summary>
	///     Begins tracking the given entities and entries reachable from the given entities using
	///     the <see cref="EntityState.Unchanged" /> state by default, but see below for cases
	///     when a different state will be used.
	/// </summary>
	/// <remarks>
	///     <para>
	///         Generally, no database interaction will be performed until <see cref="Microsoft.EntityFrameworkCore.DbContext.SaveChanges()" /> is called.
	///     </para>
	///     <para>
	///         A recursive search of the navigation properties will be performed to find reachable entities
	///         that are not already being tracked by the context. All entities found will be tracked
	///         by the context.
	///     </para>
	///     <para>
	///         For entity types with generated keys if an entity has its primary key value set
	///         then it will be tracked in the <see cref="EntityState.Unchanged" /> state. If the primary key
	///         value is not set then it will be tracked in the <see cref="EntityState.Added" /> state.
	///         This helps ensure only new entities will be inserted.
	///         An entity is considered to have its primary key value set if the primary key property is set
	///         to anything other than the CLR default for the property type.
	///     </para>
	///     <para>
	///         For entity types without generated keys, the state set is always <see cref="EntityState.Unchanged" />.
	///     </para>
	///     <para>
	///         Use <see cref="EntityEntry.State" /> to set the state of only a single entity.
	///     </para>
	///     <para>
	///         See <see href="https://aka.ms/efcore-docs-change-tracking">EF Core change tracking</see>
	///         and <see href="https://aka.ms/efcore-docs-attach-range">Using AddRange, UpdateRange, AttachRange, and RemoveRange</see>
	///         for more information and examples.
	///     </para>
	/// </remarks>
	/// <param name="entities">The entities to attach.</param>
	void AttachRange(IEnumerable<TEntity> entities);
}