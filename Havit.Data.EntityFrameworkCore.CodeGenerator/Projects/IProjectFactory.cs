namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Projects;

public interface IProjectFactory
{
	IProject Create(string folderOrCsprojPath);
}