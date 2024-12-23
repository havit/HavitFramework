namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

public interface IProjectFactory
{
	IProject Create(string folderOrCsprojPath);
}