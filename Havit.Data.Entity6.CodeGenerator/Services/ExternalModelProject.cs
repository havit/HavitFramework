namespace Havit.Data.Entity.CodeGenerator.Services;

/// <summary>
/// Model, který není projektem v solution, ale přichází z NuGet balíčku.
/// Poskytuje pouze root namespace modelu (ten je potřeba pro odvození namespaců generovaných tříd v DataLayeru),
/// zápis souborů nepodporuje.
/// </summary>
public class ExternalModelProject : IProject
{
	private readonly string rootNamespace;

	public ExternalModelProject(string rootNamespace)
	{
		this.rootNamespace = rootNamespace;
	}

	/// <summary>
	/// Externí model není reprezentován souborem projektu.
	/// </summary>
	public string Filename => null;

	public string GetProjectRootNamespace()
	{
		return rootNamespace;
	}

	public string GetProjectRootPath()
	{
		throw new NotSupportedException("Externí model není umístěn v solution, cestu k němu nelze určit.");
	}

	public void AddOrUpdate(string filename)
	{
		throw new NotSupportedException("Do externího modelu nelze generovat soubory, nastavte v konfiguraci GenerateMetadata na false.");
	}

	/// <summary>
	/// Do externího modelu se negeneruje, žádné pozůstalé soubory tedy neevidujeme.
	/// </summary>
	public string[] GetUnusedGeneratedFiles()
	{
		return Array.Empty<string>();
	}

	public void RemoveUnusedGeneratedFiles()
	{
		// NOOP, viz GetUnusedGeneratedFiles
	}

	public void SaveChanges()
	{
		// NOOP, externí model neměníme
	}
}
