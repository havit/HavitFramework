using System.Text.Json;

namespace Havit.Data.Entity.CodeGenerator.Configuration;

/// <summary>
/// Konfigurace generátoru, volitelně načtená ze souboru entity6.codegenerator.json.
/// Není-li soubor nalezen, použijí se výchozí hodnoty, tj. chování shodné se stavem před zavedením konfigurace.
/// </summary>
public class CodeGeneratorConfiguration
{
	/// <summary>
	/// Root namespace modelu pro případ, kdy model není projektem v solution, ale přichází z NuGet balíčku.
	/// Je-li vyplněn, projekt Model\Model.csproj se nehledá a do modelu se negeneruje
	/// (je tedy třeba nastavit též <see cref="GenerateMetadata"/> na false).
	/// </summary>
	public string ModelRootNamespace { get; set; }

	/// <summary>
	/// Umožní vypnout generování metadata tříd do projektu modelu.
	/// Nastavujeme na false, pokud model přichází z NuGet balíčku, který metadata třídy již obsahuje.
	/// </summary>
	public bool GenerateMetadata { get; set; } = true;

	/// <summary>
	/// Výchozí konfigurace, tj. model je projektem v solution a metadata tříd se generují.
	/// </summary>
	public static CodeGeneratorConfiguration Defaults => new CodeGeneratorConfiguration();

	/// <summary>
	/// Načte konfiguraci ze souboru. Vlastnosti, které nejsou v konfiguračním souboru uvedeny, si ponechají výchozí hodnoty.
	/// </summary>
	public static CodeGeneratorConfiguration ReadFromFile(string configurationFileName)
	{
#pragma warning disable CA1869 // Cache and reuse 'JsonSerializerOptions' instances
		var options = new JsonSerializerOptions
		{
			ReadCommentHandling = JsonCommentHandling.Skip,
			AllowTrailingCommas = true
		};
#pragma warning restore CA1869 // Cache and reuse 'JsonSerializerOptions' instances

		using (var configurationStream = File.OpenRead(configurationFileName))
		{
			return JsonSerializer.Deserialize<CodeGeneratorConfiguration>(configurationStream, options);
		}
	}
}
