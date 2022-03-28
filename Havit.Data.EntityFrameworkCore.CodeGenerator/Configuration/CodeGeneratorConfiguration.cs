using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Configuration
{
    public class CodeGeneratorConfiguration
    {
        public string ModelProjectPath { get; set; } = @"Model\Model.csproj";
        public string MetadataProjectPath { get; set; } = @"Model\Model.csproj";

        public static CodeGeneratorConfiguration Defaults => new CodeGeneratorConfiguration();

        public static CodeGeneratorConfiguration ReadFromFile(string configurationFileName)
        {
            var options = new JsonSerializerOptions
            {
                ReadCommentHandling = JsonCommentHandling.Skip,                
                AllowTrailingCommas = true
            };

            using (var configurationStream = File.OpenRead(configurationFileName))
            {
                // vlastnosti, které nejsou uvedeny v konfiguračním souboru, si ponechají výchozí hodnoty
                return System.Text.Json.JsonSerializer.Deserialize<CodeGeneratorConfiguration>(configurationStream, options);
            }
        }
    }
}
