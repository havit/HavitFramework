using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions;

/// <summary>
/// Generates necessary CREATE, ALTER or DROP SQL scripts for <see cref="IModelExtension"/> defined in <see cref="IModel"/> (stored as <see cref="IAnnotation"/>s).
/// </summary>
public class ModelExtensionMigrationOperationSqlGenerator : MigrationOperationSqlGenerator
{
	private readonly IModelExtensionAnnotationProvider modelExtensionAnnotationProvider;
	private readonly IModelExtensionSqlResolver modelExtensionSqlResolver;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public ModelExtensionMigrationOperationSqlGenerator(
		IModelExtensionAnnotationProvider modelExtensionAnnotationProvider,
		IModelExtensionSqlResolver modelExtensionSqlResolver)
	{
		this.modelExtensionAnnotationProvider = modelExtensionAnnotationProvider;
		this.modelExtensionSqlResolver = modelExtensionSqlResolver;
	}

	/// <inheritdoc />
	public override void Generate(AlterDatabaseOperation operation, IModel model, MigrationCommandListBuilder builder)
	{
		// WARNING: if this implementation (generate SQL from annotations) changes, update or rewrite tests
		var oldAnnotations = GetAnnotations(operation.OldDatabase.GetAnnotations());
		var currentAnnotations = GetAnnotations(operation.GetAnnotations());
		var newAnnotations = currentAnnotations.Where(x => !oldAnnotations.ContainsKey(x.Key)).Select(x => x.Value).ToList<IAnnotation>();
		List<IAnnotation> deletedAnnotations = oldAnnotations.Where(x => !currentAnnotations.ContainsKey(x.Key)).Select(x => x.Value).ToList<IAnnotation>();
		List<IAnnotation> existingAnnotations = oldAnnotations.Where(x => currentAnnotations.ContainsKey(x.Key)).Select(x => currentAnnotations[x.Key]).ToList<IAnnotation>();

		GenerateCreateCommands(newAnnotations, builder);
		GenerateAlterCommands(existingAnnotations, builder);
		GenerateDropCommands(deletedAnnotations, builder);
	}

	private Dictionary<string, Annotation> GetAnnotations(IEnumerable<Annotation> annotations)
	{
		return annotations.Where(a => modelExtensionAnnotationProvider.GetModelExtensions(new List<IAnnotation> { a }).Any())
			.ToDictionary(a => a.Name);
	}

	private void GenerateCreateCommands(List<IAnnotation> newAnnotations, MigrationCommandListBuilder builder)
	{
		foreach (var annotation in newAnnotations)
		{
			var sql = (string)annotation.Value;

			builder.Append(sql).AppendLine().EndCommand();
		}
	}

	private void GenerateDropCommands(List<IAnnotation> oldAnnotations, MigrationCommandListBuilder builder)
	{
		List<IModelExtension> modelExtensions = modelExtensionAnnotationProvider.GetModelExtensions(oldAnnotations);

		List<string> scripts = modelExtensionSqlResolver.ResolveDropSqlScripts(modelExtensions);

		GenerateSqlStatements(builder, scripts);
	}

	private void GenerateAlterCommands(List<IAnnotation> existingAnnotations, MigrationCommandListBuilder builder)
	{
		List<IModelExtension> modelExtensions = modelExtensionAnnotationProvider.GetModelExtensions(existingAnnotations);

		List<string> scripts = modelExtensionSqlResolver.ResolveAlterSqlScripts(modelExtensions);

		GenerateSqlStatements(builder, scripts);
	}

	private static void GenerateSqlStatements(MigrationCommandListBuilder builder, List<string> scripts)
	{
		foreach (string sql in scripts)
		{
			builder.Append(sql).AppendLine().EndCommand();
		}
	}
}