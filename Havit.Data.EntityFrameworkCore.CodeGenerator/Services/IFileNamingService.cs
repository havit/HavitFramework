namespace Havit.Data.Entity.CodeGenerator.Services
{
	public interface IFileNamingService<TModel>
	{
		string GetFilename(TModel model);
	}
}