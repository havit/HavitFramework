namespace Havit.Data.Entity.CodeGenerator.Services.SourceControl;

public interface ISourceControlClient
{
	void Add(string path);

	void Delete(string path);

	void Delete(string[] paths);
}
