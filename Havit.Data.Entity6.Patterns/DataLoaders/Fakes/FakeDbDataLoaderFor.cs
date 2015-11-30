using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Havit.Data.Patterns.Attributes;
using Havit.Data.Patterns.DataLoaders;

namespace Havit.Data.Entity.Patterns.DataLoaders.Fakes
{
	/// <summary>
	/// Prázdná implementace interface <see cref="IDbDataLoaderFor{TEntity}" /> a <see cref="IDbDataLoaderForAsync{TEntity}" />, která efektivně nevykonává žádnou činnost.
	/// Určeno pro použití v unit testech.
	/// </summary>
	[Fake]
	public class FakeDbDataLoaderFor<TEntity> : IDbDataLoaderFor<TEntity>, IDbDataLoaderForAsync<TEntity>
		where TEntity : class
	{
		/// <summary>
		/// Nic nedělá. Určeno pro použití v unit testech.
		/// </summary>
		public IDbDataLoaderFor<TProperty> Load<TProperty>(Expression<Func<TEntity, IEnumerable<TProperty>>> propertyPath)
			where TProperty : class
		{
			return new FakeDbDataLoaderFor<TProperty>();
		}

		/// <summary>
		/// Nic nedělá. Určeno pro použití v unit testech.
		/// </summary>
		public IDbDataLoaderFor<TProperty> Load<TProperty>(Expression<Func<TEntity, List<TProperty>>> propertyPath)
			where TProperty : class
		{
			return new FakeDbDataLoaderFor<TProperty>();
		}

		/// <summary>
		/// Nic nedělá. Určeno pro použití v unit testech.
		/// </summary>
		public IDbDataLoaderFor<TProperty> Load<TProperty>(Expression<Func<TEntity, ICollection<TProperty>>> propertyPath)
			where TProperty : class
		{
			return new FakeDbDataLoaderFor<TProperty>();
		}

		/// <summary>
		/// Nic nedělá. Určeno pro použití v unit testech.
		/// </summary>
		public IDbDataLoaderFor<TProperty> Load<TProperty>(Expression<Func<TEntity, TProperty>> propertyPath)
			where TProperty : class
		{
			return new FakeDbDataLoaderFor<TProperty>();
		}

		/// <summary>
		/// Nic nedělá. Určeno pro použití v unit testech.
		/// </summary>
		public IDbDataLoaderFor<TEntity> Where(Func<TEntity, bool> predicate)
		{
			return new FakeDbDataLoaderFor<TEntity>();
		}

		/// <summary>
		/// Nic nedělá. Určeno pro použití v unit testech.
		/// </summary>
		public Task<IDbDataLoaderForAsync<TProperty>> LoadAsync<TProperty>(Expression<Func<TEntity, TProperty>> propertyPath)
			where TProperty : class
		{
			return Task.FromResult((IDbDataLoaderForAsync<TProperty>)new FakeDbDataLoaderFor<TProperty>());
		}

		/// <summary>
		/// Nic nedělá. Určeno pro použití v unit testech.
		/// </summary>
		public Task<IDbDataLoaderForAsync<TProperty>> LoadAsync<TProperty>(Expression<Func<TEntity, IEnumerable<TProperty>>> propertyPath)
			where TProperty : class
		{
			return Task.FromResult((IDbDataLoaderForAsync<TProperty>)new FakeDbDataLoaderFor<TProperty>());
		}

		/// <summary>
		/// Nic nedělá. Určeno pro použití v unit testech.
		/// </summary>
		public Task<IDbDataLoaderForAsync<TProperty>> LoadAsync<TProperty>(Expression<Func<TEntity, ICollection<TProperty>>> propertyPath)
			where TProperty : class
		{
			return Task.FromResult((IDbDataLoaderForAsync<TProperty>)new FakeDbDataLoaderFor<TProperty>());
		}

		/// <summary>
		/// Nic nedělá. Určeno pro použití v unit testech.
		/// </summary>
		public Task<IDbDataLoaderForAsync<TProperty>> LoadAsync<TProperty>(Expression<Func<TEntity, List<TProperty>>> propertyPath)
			where TProperty : class
		{
			return Task.FromResult((IDbDataLoaderForAsync<TProperty>)new FakeDbDataLoaderFor<TProperty>());
		}

		/// <summary>
		/// Nic nedělá. Určeno pro použití v unit testech.
		/// </summary>
		IDbDataLoaderForAsync<TEntity> IDbDataLoaderForAsync<TEntity>.Where(Func<TEntity, bool> predicate)
		{
			// žádná data nedržíme, nic neděláme, proto zde nic nefiltrujeme
			return new FakeDbDataLoaderFor<TEntity>();
		}
	}
}
