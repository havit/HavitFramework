namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

public interface IProject
{
	string Filename { get; }

	string GetProjectRootNamespace();

	string GetProjectRootPath();
}