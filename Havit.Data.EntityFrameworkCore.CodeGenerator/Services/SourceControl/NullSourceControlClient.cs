using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.Entity.CodeGenerator.Services.SourceControl
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
