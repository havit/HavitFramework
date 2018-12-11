using System;
using Havit.Data.EntityFrameworkCore;
using Havit.EFCoreTests.Model;
using Microsoft.EntityFrameworkCore;

namespace Havit.EFCoreTests.Entity
{
	public class ApplicationDbContext : Havit.Data.EntityFrameworkCore.DbContext
	{
		/// <summary>
		/// Konstruktor.
		/// Pro použití v unit testech, jiné použití nemá.
		/// </summary>
		internal ApplicationDbContext()
		{
			// NOOP
		}

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public ApplicationDbContext(DbContextOptions options) : base(options)
		{
			// NOOP
		}

		/// <inheritdoc />
		protected override void CustomizeModelCreating(ModelBuilder modelBuilder)
		{
			base.CustomizeModelCreating(modelBuilder);

			modelBuilder.RegisterModelFromAssembly(typeof(Havit.EFCoreTests.Model.Localizations.Language).Assembly);
			modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);

			modelBuilder.Entity<FlagClass>().Property(fc => fc.MyFlag).HasDefaultValue(true).ValueGeneratedNever();
		}
	}
}
