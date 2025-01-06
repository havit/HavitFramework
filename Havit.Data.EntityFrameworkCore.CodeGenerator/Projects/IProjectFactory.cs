namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Projects;

public interface IProjectFactory
{
	TProject Create<TProject>(string folderOrCsprojPath)
		where TProject : ProjectBase, new();
}