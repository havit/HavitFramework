
using System.Text.Json;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Tool
{
	/// <summary>
	/// Podmnožina konfigurace CodeGeneratoru načítaná z JSON.
	/// </summary>
	public class CodeGeneratorLimitedConfiguration
	{
		public string SolutionDirectory { get; set; }
		public string EntityProjectPath { get; set; }

		public static bool TryGetConfiguration(out CodeGeneratorLimitedConfiguration result)
		{
			string configurationFileName = Path.Combine(System.Environment.CurrentDirectory, "efcore.codegenerator.json");
			if (File.Exists(configurationFileName))
			{
				result = CodeGeneratorLimitedConfiguration.ReadFromFile(configurationFileName);
				return true;
			}


			result = null;
			return false;
		}

		private static CodeGeneratorLimitedConfiguration ReadFromFile(string configurationFileName)
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
				return System.Text.Json.JsonSerializer.Deserialize<CodeGeneratorLimitedConfiguration>(configurationStream, options);
			}
		}

	}
}