using System.Text.Json;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Configuration;

public class CodeGeneratorConfiguration
{
	[Obsolete("Used only by code generator tool.", error: true)]
	public string SolutionDirectory { get; set; }

	// Z důvodu zpětné kompatibility (jak CodeGeneratorToolu s CodeGeneratorem, tak údajů v konfiguracích jednotlivých projektů
	// jsou následující cesty relativní vůči SolutionDirectory.

	[Obsolete("Used only by code generator tool.", error: true)]
	public string EntityProjectPath { get; set; }

	public string ModelProjectPath { get; set; } = Path.Combine("Model", "Model.csproj");
	public string MetadataProjectPath { get; set; } = Path.Combine("Model", "Model.csproj");
	public string DataLayerProjectPath { get; set; } = Path.Combine("DataLayer", "DataLayer.csproj");
	public string MetadataNamespace { get; set; } = @"Metadata";

	/// <summary>
	/// Umožní potlačit mazání repositories při čištění pozůstalých souborů.
	/// </summary>
	public bool SuppressRemovingRelicRepositories { get; set; } = false;

	public static CodeGeneratorConfiguration Defaults => new CodeGeneratorConfiguration();

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
			// vlastnosti, které nejsou uvedeny v konfiguračním souboru, si ponechají výchozí hodnoty
			return System.Text.Json.JsonSerializer.Deserialize<CodeGeneratorConfiguration>(configurationStream, options);
		}
	}
}
