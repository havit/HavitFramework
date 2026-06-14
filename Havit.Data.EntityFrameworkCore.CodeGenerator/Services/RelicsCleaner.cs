
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
	/// Budeme mazat vše, co je ve složkách _generated, ale nebylo aktuálně vygenerováno (pozůstatky metadat, datasources, repositories vč. query provideru, atp.)
	/// Dále budeme mazat soubory ve složce Repositories v projektu DataLayer, které odpovídají vzoru IXyRepository.cs a XyDbRepository.cs, které nebyly aktuálně vygenerovány.
	/// </summary>
	public Task CleanRelicsAsync(CancellationToken cancellationToken)
	{
		// všechny soubory, které byly aplikací právě vygenerovány
		List<string> allWrittenFiles = _codeWriteReporter.GetWrittenFiles();

		// všechny soubory, které byly (i dříve) vygenerovány ve složkách _generated projektů DataLayer a ModelMetadata
		List<string> allFilesInGeneratedFolders = GetFilesInGeneratedFolder(_dataLayerProject)
			.Concat(GetFilesInGeneratedFolder(_metadataProject))
			.ToList();

		// Všechny soubory, které byly (i dříve) vygenerovány v projektu DataLayer ve složce Repositories,
		// které odpovídají vzoru Repositories v projektu DataLayer, které odpovídají vzoru IXyRepository.cs a XyDbRepository.cs.
		List<string> allRepositories = _configuration.SuppressRemovingRelicRepositories
			? new List<string>() // pokud máme potlačit mazání relic souborů repository, nebudeme je ani hledat
			: GetRepositoryFiles();

		// StringComparer.CurrentCultureIgnoreCase: Teoreticky může být na disku (historický, avšak stále aktivní) soubor repository s jinak
		// case-sensitive názvem, než je současný název. Názvy souborů se snažíme korigovat, mohou však zůstat jinak pojmenované složky.
		// Soubory, které se liší jen velikostí písmen, proto nechceme odstraňovat.
		var relicFiles = allFilesInGeneratedFolders.Concat(allRepositories).Except(allWrittenFiles, StringComparer.CurrentCultureIgnoreCase).ToList();
		if (relicFiles.Count > 0)
		{
			Console.WriteLine($"Removing {relicFiles.Count} relic file(s)...");
			foreach (var relicFile in relicFiles)
			{
				File.Delete(relicFile);
			}
		}

		// Po smazání pozůstalých souborů odstraníme i adresáře, které tím zůstaly prázdné.
		RemoveEmptySubdirectories(Path.Combine(_dataLayerProject.GetProjectRootPath(), "_generated"));
		RemoveEmptySubdirectories(Path.Combine(_metadataProject.GetProjectRootPath(), "_generated"));
		if (!_configuration.SuppressRemovingRelicRepositories)
		{
			RemoveEmptySubdirectories(Path.Combine(_dataLayerProject.GetProjectRootPath(), "Repositories"));
		}

		return Task.CompletedTask;
	}

	private static List<string> GetFilesInGeneratedFolder(IProject project)
	{
		var generatedProjectSubfolder = Path.Combine(project.GetProjectRootPath(), "_generated");
		return Directory.EnumerateFiles(generatedProjectSubfolder, "*.*", SearchOption.AllDirectories).ToList();
	}

	private List<string> GetRepositoryFiles()
	{
		var repositoriesFolder = Path.Combine(_dataLayerProject.GetProjectRootPath(), "Repositories");
		List<string> generatedRepositoryInterfacesFiles = null;
		List<string> generatedRepositoryImplementationFiles = null;
		Parallel.Invoke(
			() => generatedRepositoryInterfacesFiles = Directory.EnumerateFiles(repositoriesFolder, "I*Repository.cs", SearchOption.AllDirectories).ToList(),
			() => generatedRepositoryImplementationFiles = Directory.EnumerateFiles(repositoriesFolder, "*DbRepository.cs", SearchOption.AllDirectories).ToList());

		return generatedRepositoryInterfacesFiles.Concat(generatedRepositoryImplementationFiles).ToList();
	}

	/// <summary>
	/// Rekurzivně (zdola nahoru) odstraní prázdné podadresáře ve složce <paramref name="rootFolder"/>. Samotnou <paramref name="rootFolder"/> ponechá.
	/// </summary>
	private static void RemoveEmptySubdirectories(string rootFolder)
	{
		if (!Directory.Exists(rootFolder))
		{
			return;
		}

		foreach (string subdirectory in Directory.GetDirectories(rootFolder))
		{
			RemoveEmptySubdirectories(subdirectory);
			if (!Directory.EnumerateFileSystemEntries(subdirectory).Any())
			{
				Directory.Delete(subdirectory);
			}
		}
	}

}
