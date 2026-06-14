using Havit.Data.EntityFrameworkCore.CodeGenerator.Services;
using Havit.Data.EntityFrameworkCore.Metadata;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Configuration;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Projects;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.ModelMetadataClasses.Model;

public class MetadataClassModelSource : IModelSource<MetadataClass>
{
	private readonly DbContext _dbContext;
	private readonly IMetadataProject _metadataProject;
	private readonly IModelProject _modelProject;
	private readonly CodeGeneratorConfiguration _configuration;

	// Tento model source je DI singleton sdílený mezi generátory, které běží paralelně (viz DataLayerGeneratorRunner.Parallel.ForEachAsync).
	// Lazy (výchozí ExecutionAndPublication) zajistí, že se modely vyhodnotí právě jednou i při souběžném přístupu z více generátorů.
	private readonly Lazy<List<MetadataClass>> _models;

	public MetadataClassModelSource(DbContext dbContext, IMetadataProject metadataProject, IModelProject modelProject, CodeGeneratorConfiguration configuration)
	{
		_dbContext = dbContext;
		_metadataProject = metadataProject;
		_modelProject = modelProject;
		_configuration = configuration;
		_models = new Lazy<List<MetadataClass>>(GetModelsCore);
	}

	public List<MetadataClass> GetModels() => _models.Value;

	private List<MetadataClass> GetModelsCore()
	{
		return (from registeredEntity in _dbContext.Model.GetApplicationEntityTypes(includeManyToManyEntities: false)
				select new MetadataClass
				{
					NamespaceName = GetNamespaceName(registeredEntity.ClrType.Namespace),
					ClassName = registeredEntity.ClrType.Name + "Metadata",
					MaxLengthConstants = (from property in registeredEntity.GetProperties()
										  where property.ClrType == typeof(string)
										  select new MetadataClass.MaxLengthConstant
										  {
											  Name = property.Name + "MaxLength",
											  // Pokud je použit MaxLengthAttribute atribut bez hodnoty, je jako hodnota považována -1.
											  // Konvence MaxLengthAttributeConvention aplikující MaxLengthAttributy, použije jen atributy s hodnotou > 0.
											  // Property, bez nastavené hodnoty (null), jsou považovány za nvarchar(max). Stejně tak i property s nastavenou maximální délkou na Int32.MaxValue.
											  Value = ((property.GetMaxLength() == null) || (property.GetMaxLength() == Int32.MaxValue))
												  ? "Int32.MaxValue"
												  : property.GetMaxLength().ToString()
										  })
						.OrderBy(property => property.Name, StringComparer.InvariantCulture)
						.ToList()
				}).ToList();
	}

	private string GetNamespaceName(string namespaceName)
	{
		string metadataProjectNamespace = _metadataProject.GetProjectRootNamespace();
		string modelProjectNamespace = _modelProject.GetProjectRootNamespace();
		if (NamespaceHelper.TryGetRelativeNamespace(namespaceName, modelProjectNamespace, out string relativeNamespace))
		{
			return metadataProjectNamespace + "." + _configuration.MetadataNamespace + relativeNamespace;
		}
		else
		{
			return namespaceName + "." + _configuration.MetadataNamespace;
		}
	}
}
