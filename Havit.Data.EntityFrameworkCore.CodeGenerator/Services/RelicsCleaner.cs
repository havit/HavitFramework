
using Havit.Data.EntityFrameworkCore.CodeGenerator.Configuration;
using Havit.Data.EntityFrameworkCore.CodeGenerator.Projects;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

public class RelicsCleaner : IRelicsCleaner
{
	private readonly IDataLayerProject _dataLayerProject;
	private readonly IMetadataProject _metadataProject;
	private readonly ICodeWriteReporter _codeWriteReporter;
	private readonly CodeGeneratorConfiguration _configuration;

	public RelicsCleaner(IDataLayerProject dataLayerProject, IMetadataProject metadataProject, ICodeWriteReporter codeWriteReporter, CodeGeneratorConfiguration configuration)
	{
		_dataLayerProject = dataLayerProject;
		_metadataProject = metadataProject;
		_codeWriteReporter = codeWriteReporter;
		_configuration = configuration;
	}

	/// <summary>
	/// Budeme mazat vše, co je ve složkách _generated, ale nebylo aktuálně vygenerováno (pozůstatky metadat, datasources, reporitories vč. query provideru, atp.)
	/// Dále budeme mazat soubory ve složce Repositories v projektu DataLayer, které odpovídají vzoru IXyRepository.cs a XyDbRepository.cs, které nebyly aktuálně vygenerovány.
	/// </summary>
	public async Task CleanRelicsAsync(CancellationToken cancellationToken)
	{
		// všechny soubory, které byly aplikací právě vygenerovány
		List<string> allWrittenFiles = _codeWriteReporter.GetWrittenFiles();

		// všechny soubory, které byly (i dříve) vygenerovány ve složkách _generated projektů DataLayer a ModelMetadata
		List<string> allFilesInGeneratedFolders = (await GetFilesInGeneratedFolderAsync(_dataLayerProject, cancellationToken))
			.Concat(await GetFilesInGeneratedFolderAsync(_metadataProject, cancellationToken))
			.ToList();

		// Všechny soubory, které byly (i dříve) vygenerovány v projektu DataLayer ve složce Repositories,
		// které odpovídají vzoru Repositories v projektu DataLayer, které odpovídají vzoru IXyRepository.cs a XyDbRepository.cs.
		List<string> allRepositories = _configuration.SuppressRemovingRelicRepositories
			? new List<string>() // pokud máme potlačit mazání relic souborů repository, nebudeme je ani hledat
			: await GetRepositoryFilesAsync(cancellationToken);

		var relicFiles = allFilesInGeneratedFolders.Concat(allRepositories).Except(allWrittenFiles).ToList();
		if (relicFiles.Count > 0)
		{
			Console.WriteLine($"Removing {relicFiles.Count} relic file(s)...");
			foreach (var relicFile in relicFiles)
			{
				File.Delete(relicFile);
			}
		}
	}

	private Task<List<string>> GetFilesInGeneratedFolderAsync(IProject project, CancellationToken cancellationToken)
	{
		var generatedProjectSubfolder = Path.Combine(project.GetProjectRootPath(), "_generated");
		var generatedFiles = Directory.EnumerateFiles(generatedProjectSubfolder, "*.*", SearchOption.AllDirectories).ToList();
		return Task.FromResult(generatedFiles);
	}

	private Task<List<string>> GetRepositoryFilesAsync(CancellationToken cancellationToken)
	{
		var repositoriesFolder = Path.Combine(_dataLayerProject.GetProjectRootPath(), "Repositories");
		List<string> generatedRepositoryInterfacesFiles = null;
		List<string> generatedRepositoryImplementationFiles = null;
		Parallel.Invoke(
			() => generatedRepositoryInterfacesFiles = Directory.EnumerateFiles(repositoriesFolder, "I*Repository.cs", SearchOption.AllDirectories).ToList(),
			() => generatedRepositoryImplementationFiles = Directory.EnumerateFiles(repositoriesFolder, "*DbRepository.cs", SearchOption.AllDirectories).ToList());

		return Task.FromResult(generatedRepositoryInterfacesFiles.Concat(generatedRepositoryImplementationFiles).ToList());
	}

}
