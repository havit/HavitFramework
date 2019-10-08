using Havit.Data.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore
{
	/// <summary>
	/// Extension metody k DbContextOptionsBuilder.
	/// </summary>
	public static class DbContextOptionsBuilderExtensions
	{
		/// <summary>
		/// Zajistí použití dané implementace IConventionSetPlugin.
		/// </summary>
		/// <remarks>
		/// Jak to funguje:
		/// - Pro instalaci konvencí potřebujeme do service collection dostat třídu implementující IConventionSetPlugin, tato třída bude nastavovat ConventionSet.
		/// - Jinými slovy, jednotlivé konvence nejsou v service collection, avšak jsou tam třídy, které konvence instalují do ConventionSetu.
		/// 
		/// - DbContext v OnConfiguring získává DbContextOptionsBuilder.
		/// - Tento DbContextOptionsBuilder (bohužel explicitně) implementuje interface IDbContextOptionsBuilderInfrastructure.
		/// - Tento intarface poskytuje metodu pro zaregistrování extension - třídy, službičky, která má možnost ovlivnit ServiceCollection DbContextu.
		/// - ConventionSetPluginServiceInstallerExtension registrujeme pod IConventionSetPlugin do service collection.
		/// </remarks>
		public static void UseConventionSetPlugin<TConventionSetPlugin>(this DbContextOptionsBuilder optionsBuilder)
			where TConventionSetPlugin : class, IConventionSetPlugin
		{
			((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(new ConventionSetPluginServiceInstallerExtension<TConventionSetPlugin>());
		}
	}
}
