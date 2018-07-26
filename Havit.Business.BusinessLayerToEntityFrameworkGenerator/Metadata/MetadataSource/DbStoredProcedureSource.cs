using System.Collections.Generic;
using System.Linq;
using Havit.Business.BusinessLayerGenerator.Helpers;
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

            return dbProcedures;
        }

        private DbStoredProcedure CreateStoredProcedure(StoredProcedure storedProcedure, GeneratedModel model)
        {
            (string tableName, string name) = ParseStoredProcedureName(storedProcedure.Name);
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
                StoredProcedure = storedProcedure
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