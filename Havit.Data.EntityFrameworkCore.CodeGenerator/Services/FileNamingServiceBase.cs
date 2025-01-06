using Havit.Data.EntityFrameworkCore.CodeGenerator.Projects;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

public abstract class FileNamingServiceBase<TModel> : IFileNamingService<TModel>
{
	private readonly IProject _project;

	protected FileNamingServiceBase(IProject project)
	{
		_project = project;
	}

	protected virtual bool UseGeneratedFolder
	{
		get { return true; }
	}

	protected abstract string GetClassName(TModel model);
	protected abstract string GetNamespaceName(TModel model);

	public virtual string GetFilename(TModel model)
	{
		string namespaceName = GetNamespaceName(model);
		bool useGeneratedFolder = UseGeneratedFolder;
		string className = GetClassName(model);

		string projectRootPath = _project.GetProjectRootPath();
		string projectRootNamespace = _project.GetProjectRootNamespace();

		string namespaceFolder;
		if (namespaceName.StartsWith(projectRootNamespace))
		{
			namespaceFolder = namespaceName.Substring(projectRootNamespace.Length).Trim('.').Replace('.', Path.DirectorySeparatorChar);
		}
		else
		{
			namespaceFolder = namespaceName.Replace('.', Path.DirectorySeparatorChar);
		}

		string classFilename = className + ".cs";

		return Path.Combine(
			projectRootPath,
			useGeneratedFolder ? "_generated" : "",
			namespaceFolder,
			classFilename);
	}
}
