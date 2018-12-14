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
			modelBuilder.Entity<ClassWithDefaults>(cb =>
			{
				cb.Property(fc => fc.DateTimeValue).HasDefaultValue(new DateTime(2018, 12, 24)).ValueGeneratedNever();
				cb.Property(fc => fc.StringValue).HasDefaultValue("ABC").ValueGeneratedNever();
				cb.Property(fc => fc.IntValue).HasDefaultValue(0).ValueGeneratedNever();
				cb.Property(fc => fc.BoolValue).HasDefaultValue(true).ValueGeneratedNever();
			});
		}
	}
}
