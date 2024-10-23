using System.Reflection;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Havit.Data.EntityFrameworkCore.Migrations;

/// <summary>
/// Extension metody pre použití v migracích.
/// </summary>
public static class MigrationExtensions
{
	/// <summary>
	/// Spustí v rámci migrace SQL skript z Embedded Resources.
	/// </summary>
	/// <param name="migrationBuilder"><see cref="MigrationBuilder"/></param>
	/// <param name="resourceName">Název resource, který je vyhledán v resources dodané assembly a následně předán ke spuštění v migraci.</param>
	/// <param name="sqlResourceAssembly">Assembly, ve které je hledán resource.</param>
	public static void SqlResource(this MigrationBuilder migrationBuilder, string resourceName, Assembly sqlResourceAssembly)
	{
		ArgumentNullException.ThrowIfNull(resourceName);

		using var stream = sqlResourceAssembly.GetManifestResourceStream(resourceName);
		if (stream == null)
		{
			throw new ArgumentException($"Resource name '{resourceName}' does not exist");
		}

		string sql;
		using (var textStream = new StreamReader(stream))
		{
			sql = textStream.ReadToEnd();
		}
		migrationBuilder.Sql(sql);
	}
}
