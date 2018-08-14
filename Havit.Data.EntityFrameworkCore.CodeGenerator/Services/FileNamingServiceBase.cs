using System.IO;

namespace Havit.Data.Entity.CodeGenerator.Services
{
	public abstract class FileNamingServiceBase<TModel> : IFileNamingService<TModel>
	{
		private readonly IProject project;

		protected FileNamingServiceBase(IProject project)
		{
			this.project = project;
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
			bool useGeneratedFolder = this.UseGeneratedFolder;
			string className = GetClassName(model);

			string projectRootPath = project.GetProjectRootPath();
			string projectRootNamespace = project.GetProjectRootNamespace();

			string namespaceFolder;
			if (namespaceName.StartsWith(projectRootNamespace))
			{
				namespaceFolder = namespaceName.Substring(projectRootNamespace.Length).Trim('.').Replace(".", @"\");
			}
			else
			{
				namespaceFolder = namespaceName.Replace(".", @"\");
			}

			string classFilename = className + ".cs";

			return Path.Combine(
				projectRootPath,
				useGeneratedFolder ? "_generated" : "",
				namespaceFolder,
				classFilename);
		}
	}
}
