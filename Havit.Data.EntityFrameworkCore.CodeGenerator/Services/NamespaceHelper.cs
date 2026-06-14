namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

/// <summary>
/// Pomocné metody pro mapování názvů namespace modelových tříd do jednotlivých vrstev (repositories, data sources, data entries, metadata, ...).
/// </summary>
internal static class NamespaceHelper
{
	/// <summary>
	/// Zjišťuje, zda <paramref name="namespaceName"/> spadá pod <paramref name="rootNamespace"/>, tj. zda se mu rovná
	/// nebo začíná na "<paramref name="rootNamespace"/>.". Porovnání je ordinální a respektuje hranice segmentů namespace,
	/// takže např. "App.Model" neodpovídá "App.ModelExtensions".
	/// </summary>
	/// <param name="relativeNamespace">
	/// Pokud <paramref name="namespaceName"/> spadá pod <paramref name="rootNamespace"/>, obsahuje zbývající část za root namespace
	/// (včetně úvodní tečky, nebo prázdný řetězec při shodě). Jinak <c>null</c>.
	/// </param>
	/// <returns><c>true</c>, pokud <paramref name="namespaceName"/> spadá pod <paramref name="rootNamespace"/>; jinak <c>false</c>.</returns>
	public static bool TryGetRelativeNamespace(string namespaceName, string rootNamespace, out string relativeNamespace)
	{
		if (String.Equals(namespaceName, rootNamespace, StringComparison.Ordinal))
		{
			relativeNamespace = String.Empty;
			return true;
		}

		if ((namespaceName.Length > rootNamespace.Length)
			&& (namespaceName[rootNamespace.Length] == '.')
			&& namespaceName.StartsWith(rootNamespace, StringComparison.Ordinal))
		{
			relativeNamespace = namespaceName.Substring(rootNamespace.Length); // včetně úvodní tečky
			return true;
		}

		relativeNamespace = null;
		return false;
	}
}
