namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Services
{
	public interface IProject
	{
		string Filename { get; }

		void AddOrUpdate(string filename);

		void RemoveUnusedGeneratedFiles();

		string[] GetUnusedGeneratedFiles();

		void SaveChanges();

		string GetProjectRootNamespace();

		string GetProjectRootPath();
	}
}