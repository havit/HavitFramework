using System.Text.RegularExpressions;

namespace Havit.Business.BusinessLayerGenerator.Helpers.NamingConventions;

public static class ConventionsHelper
{
	/// <summary>
	/// Převede řetězec na cammel case formát (cammelCase). "ID" převádí na "id".
	/// </summary>
	public static string GetCammelCase(string value)
	{
		if (value.ToLower() == "id")
		{
			return "id";
		}

		if (value.Length <= 2)
		{
			return value.ToLower();
		}

		return value.Substring(0, 1).ToLower() + value.Substring(1);
	}

	/// <summary>
	/// Vrátí hodnotu parametru doplněněnou o znak podtržítka na začátku.
	/// Pokud hodnota parametru začíná podtržítkem, vrací nezměněnou hodnotu.
	/// </summary>
	public static string GetUnderScoped(string value)
	{
		return "_" + value;
	}

	/// <summary>
	/// Vrací true, pokud je parametr platným názvem třídy.
	/// Musí začínat velkým písmenem a dále se mohou vyskytovat velká a malá písmena anglické abecedy, číslice a podtržítko.
	/// </summary>
	public static bool IsValidClassName(string value)
	{
		return Regex.IsMatch(value, @"^[A-Z][A-Za-z0-9_]*$");
	}
}
