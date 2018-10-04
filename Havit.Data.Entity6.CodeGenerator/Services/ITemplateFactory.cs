namespace Havit.Data.Entity.CodeGenerator.Services
{
	public interface ITemplateFactory<TModel>
	{
		ITemplate CreateTemplate(TModel model);
	}
}
