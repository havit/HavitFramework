using System.Collections.Generic;
using System.IO;
using System.Linq;
using Havit.Business.BusinessLayerGenerator.Helpers;
using Havit.Business.BusinessLayerToEntityFrameworkGenerator.Helpers;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerToEntityFrameworkGenerator.Metadata.MetadataSource
{
    public class DbStoredProcedureSource
    {
        public List<DbStoredProcedure> GetStoredProcedures(Database database, GeneratedModel model)
        {
            ConsoleHelper.WriteLineInfo("Vyhledávám uložené procedury");
            var procedures = database.StoredProcedures.Cast<StoredProcedure>()
                .Where(sp => !sp.IsSystemObject)
                .ToArray();

            var dbProcedures = new List<DbStoredProcedure>();
            foreach (StoredProcedure storedProcedure in procedures)
            {
                ConsoleHelper.WriteLineInfo(storedProcedure.Name);

                dbProcedures.Add(CreateStoredProcedure(storedProcedure, model));
            }

            PrefixConflictedNames(dbProcedures);

            return dbProcedures;
        }

        private void PrefixConflictedNames(List<DbStoredProcedure> dbProcedures)
        {
            var duplicatedStoredProcedures = dbProcedures.GroupBy(sp => (sp.EntityName, sp.Name))
                .Where(g => g.Count() >= 2)
                .SelectMany(g => g.Skip(1));

            foreach (DbStoredProcedure storedProcedure in duplicatedStoredProcedures)
            {
                storedProcedure.Name = storedProcedure.FullName;
            }
        }

        private DbStoredProcedure CreateStoredProcedure(StoredProcedure storedProcedure, GeneratedModel model)
        {
            string tableName = storedProcedure.GetStringExtendedProperty("Attach");
            (var _, string name) = ParseStoredProcedureName(storedProcedure.Name);
            if (tableName != null && model.GetEntityByTable(tableName) == null)
            {
                tableName = null;
                name = storedProcedure.Name;
            }

	        return new DbStoredProcedure
	        {
		        FullName = storedProcedure.Name,
		        Name = name,
		        EntityName = tableName,
		        StoredProcedure = storedProcedure,

		        GeneratedFile = Path.Combine("Entity", BusinessLayerGenerator.Helpers.FileHelper.GetFilename("Sql.StoredProcedures", storedProcedure.Name, ".sql", ""))
	        };
        }

        private (string tableName, string name) ParseStoredProcedureName(string storedProcedureName)
        {
            var split = storedProcedureName.Split('_');
            if (split.Length == 2)
            {
                return (split[0], split[1]);
            }

            return (null, split[0]);
        }
    }
}