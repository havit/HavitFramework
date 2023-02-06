using System;
using System.Collections.Generic;
using System.Reflection;
using Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions;
using Havit.Data.EntityFrameworkCore.Migrations.TestHelpers.Fakes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.Migrations.TestHelpers
{
	/// <summary>
	/// Various extension methods for <see cref="DbContextOptionsBuilder"/> used by tests.
	/// </summary>
	public static class DbContextOptionsBuilderExtensions
	{
		/// <summary>
		/// Replace <see cref="IModelExtensionsAssembly"/> with fake one, that returns types specified in <paramref name="typeInfos"/>.
		///
		/// This is used to fake out <see cref="IModelExtender"/>s and disable their automatic discovery.
		///
		/// Uses <see cref="FakeModelExtensionsAssemblyExtension"/> that replaces the service in EF Core's <see cref="IServiceProvider"/>.
		/// </summary>
		public static void SetModelExtenderTypes(this DbContextOptionsBuilder optionsBuilder, IEnumerable<TypeInfo> typeInfos)
		{
			((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(new FakeModelExtensionsAssemblyExtension(typeInfos));
		}
	}
}
