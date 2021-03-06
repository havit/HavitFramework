﻿using Havit.Data.EntityFrameworkCore.Patterns.Lookups;
using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks;
using Havit.Data.Patterns.DataSources;
using Havit.Data.Patterns.Infrastructure;
using Havit.Data.Patterns.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.Lookups.Infrastructure
{
	public class UzivatelLookupService : LookupServiceBase<string, Uzivatel>
	{

		public UzivatelLookupService(IEntityLookupDataStorage lookupStorage, IRepository<Uzivatel> repository, IDataSource<Uzivatel> dataSource, IEntityKeyAccessor entityKeyAccessor, ISoftDeleteManager softDeleteManager) : base(lookupStorage, repository, dataSource, entityKeyAccessor, softDeleteManager)
		{
		}

		/// <summary>
		/// Vyhledá uživatele podle emailu.
		/// </summary>
		public Uzivatel GetUzivatelByEmail(string email) => GetEntityByLookupKey(email);

		/// <summary>
		/// Párovací klíč je email.
		/// </summary>
		protected override Expression<Func<Uzivatel, string>> LookupKeyExpression => uzivatel => uzivatel.Email;

		protected override LookupServiceOptimizationHints OptimizationHints => LookupServiceOptimizationHints.None;

		/// <summary>
		/// IncludeDeleted nastavitelné, pro účely unit testu.
		/// </summary>
		protected override bool IncludeDeleted => includeDeleted;
		private bool includeDeleted = false;

		/// <summary>
		/// ThrowExceptionWhenNotFound nastavitelné, pro účely unit testu.
		/// </summary>
		protected override bool ThrowExceptionWhenNotFound => throwExceptionWhenNotFound;
		private bool throwExceptionWhenNotFound = true;

		/// <summary>
		/// Filter nastavitelný, pro účely unit testu.
		/// </summary>
		protected override Expression<Func<Uzivatel, bool>> Filter => filter;
		private Expression<Func<Uzivatel, bool>> filter;
		
		public void SetIncludeDeleted(bool value) => includeDeleted = value;
		public void SetThrowExceptionWhenNotFound(bool value) => throwExceptionWhenNotFound = value;
		public void SetFilter(Expression<Func<Uzivatel, bool>> value) => filter = value;

		public new void Invalidate(Changes changes) // chceme metodu Invalidate zveřejnit pro možnost použití v unit testu
		{
			base.Invalidate(changes);
		}

	}
}
