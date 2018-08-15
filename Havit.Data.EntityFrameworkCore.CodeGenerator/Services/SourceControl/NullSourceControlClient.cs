namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Services.SourceControl
{
	public class NullSourceControlClient : ISourceControlClient
	{
		public void Add(string path)
		{
			// NOOP
		}

		public void Delete(string path)
		{
			// NOOP
		}

		public void Delete(string[] paths)
		{
			// NOOP
		}
	}
}
