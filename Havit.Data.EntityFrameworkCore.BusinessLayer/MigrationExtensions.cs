using System;
using System.IO;
using System.Reflection;
using Havit.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer;

/// <summary>
/// Extension metódy pre použitie v migráciach.
/// </summary>
public static class MigrationExtensions
{
	/// <summary>
	/// Vykoná SQL skript v rámci migrácie, ktorý je načítaný z resources.
	/// </summary>
	/// <param name="migrationBuilder"><see cref="MigrationBuilder"/> používaný v migrácii.</param>
	/// <param name="resourceName">Názov resource v assembly špecifikovanej v <paramref name="sqlResourceAssembly"/>.</param>
	/// <param name="sqlResourceAssembly"><see cref="Assembly"/>, kde sa má resource hľadať.</param>
	/// <param name="suppressTransaction">Parameter pre <see cref="MigrationBuilder.Sql"/>: Indicates whether or not transactions will be suppressed while executing the SQL.</param>
	public static void SqlResource(this MigrationBuilder migrationBuilder, string resourceName, Assembly sqlResourceAssembly, bool suppressTransaction)
	{
		Contract.Requires<ArgumentNullException>(migrationBuilder != null);
		Contract.Requires<ArgumentNullException>(resourceName != null);
		Contract.Requires<ArgumentNullException>(sqlResourceAssembly != null);

		using (var stream = sqlResourceAssembly.GetManifestResourceStream(resourceName))
		{
			Contract.Assert<ArgumentException>(stream != null,
				$"Resource name '{resourceName}' does not exist.\n\nAssembly: {sqlResourceAssembly.FullName}\n\n. " +
				$"Following resources exist: \n\n{string.Join("\n", sqlResourceAssembly.GetManifestResourceNames())}");

			using (var textStream = new StreamReader(stream))
			{
				string sql = textStream.ReadToEnd();
				migrationBuilder.Sql(sql, suppressTransaction);
			}
		}
	}

	/// <summary>
	/// Vykoná SQL skript v rámci migrácie, ktorý je načítaný z resources.
	/// </summary>
	/// <param name="migrationBuilder"><see cref="MigrationBuilder"/> používaný v migrácii.</param>
	/// <param name="resourceName">Názov resource vo volajúcej assembly.</param>
	/// <param name="suppressTransaction">Parameter pre <see cref="MigrationBuilder.Sql"/>: Indicates whether or not transactions will be suppressed while executing the SQL.</param>
	public static void SqlResource(this MigrationBuilder migrationBuilder, string resourceName, bool suppressTransaction = false)
	{
		Contract.Requires<ArgumentNullException>(migrationBuilder != null);
		Contract.Requires<ArgumentNullException>(resourceName != null);

		Assembly sqlResourceAssembly = Assembly.GetCallingAssembly();

		SqlResource(migrationBuilder, resourceName, sqlResourceAssembly, suppressTransaction);
	}

	/// <summary>
	/// Vykoná SQL skript v rámci migrácie, ktorý je načítaný z resources.
	/// </summary>
	/// <param name="migration"><see cref="Migration"/> objekt. Využije sa iba na získanie reference na assembly s resource súbormi.</param>
	/// <param name="migrationBuilder"><see cref="MigrationBuilder"/> používaný v migrácii.</param>
	/// <param name="resourceName">Názov resource vo volajúcej assembly.</param>
	/// <param name="suppressTransaction">Parameter pre <see cref="MigrationBuilder.Sql"/>: Indicates whether or not transactions will be suppressed while executing the SQL.</param>
	public static void SqlResource(this Migration migration, MigrationBuilder migrationBuilder, string resourceName, bool suppressTransaction = false)
	{
		Contract.Requires<ArgumentNullException>(migration != null);

		Assembly sqlResourceAssembly = migration.GetType().Assembly;
		SqlResource(migrationBuilder, resourceName, sqlResourceAssembly, suppressTransaction);
	}
}