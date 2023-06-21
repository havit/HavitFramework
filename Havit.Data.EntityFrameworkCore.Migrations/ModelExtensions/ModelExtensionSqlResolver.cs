using System;
using System.Collections.Generic;
using System.Linq;

namespace Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions;

/// <inheritdoc />
public class ModelExtensionSqlResolver : IModelExtensionSqlResolver
{
	private readonly IEnumerable<IModelExtensionSqlGenerator> sqlGenerators;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public ModelExtensionSqlResolver(IEnumerable<IModelExtensionSqlGenerator> sqlGenerators)
	{
		this.sqlGenerators = sqlGenerators;
	}

	/// <inheritdoc />
	public List<string> ResolveAlterSqlScripts(List<IModelExtension> modelExtensions)
	{
		return CollectSqlScripts(modelExtensions, ((generator, modelExtension) => generator.GenerateAlterSql(modelExtension)));
	}

	/// <inheritdoc />
	public List<string> ResolveDropSqlScripts(List<IModelExtension> modelExtensions)
	{
		return CollectSqlScripts(modelExtensions, ((generator, modelExtension) => generator.GenerateDropSql(modelExtension)));
	}

	private List<string> CollectSqlScripts(List<IModelExtension> modelExtensions, Func<IModelExtensionSqlGenerator, IModelExtension, string> sqlProvider)
	{
		var list = new List<string>();

		foreach (IModelExtension modelExtension in modelExtensions)
		{
			IEnumerable<string> sqlStatements = sqlGenerators
				.Select(g => sqlProvider(g, modelExtension))
				.Where(s => s != null);

			list.AddRange(sqlStatements);
		}

		return list;
	}
}