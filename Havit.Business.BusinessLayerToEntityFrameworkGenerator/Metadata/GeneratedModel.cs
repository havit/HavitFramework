using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerToEntityFrameworkGenerator.Metadata
{
	public class GeneratedModel
	{
		public List<GeneratedModelClass> Entities { get; } = new List<GeneratedModelClass>();

		public GeneratedModel()
		{
		}

		public GeneratedModel(IEnumerable<GeneratedModelClass> entities)
		{
			Entities.AddRange(entities);
		}

		public GeneratedModelClass GetEntityByTable(Table table)
		{
			return Entities.FirstOrDefault(entity => entity.Table == table);
		}

		public GeneratedModelClass GetEntityByTable(string tableName)
		{
			return Entities.FirstOrDefault(entity => entity.Table.Name == tableName);
		}
	}
}