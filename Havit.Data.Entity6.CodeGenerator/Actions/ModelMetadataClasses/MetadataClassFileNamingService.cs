using Havit.Data.Entity.CodeGenerator.Actions.ModelMetadataClasses.Model;
using Havit.Data.Entity.CodeGenerator.Services;

namespace Havit.Data.Entity.CodeGenerator.Actions.ModelMetadataClasses
{
	public class MetadataClassFileNamingService : FileNamingServiceBase<MetadataClass>
	{
		public MetadataClassFileNamingService(Project project)
			: base(project)
		{
			
		}

		protected override string GetClassName(MetadataClass model)
		{
			return model.ClassName;
		}

		protected override string GetNamespaceName(MetadataClass model)
		{
			return model.NamespaceName;
		}
	}
}
