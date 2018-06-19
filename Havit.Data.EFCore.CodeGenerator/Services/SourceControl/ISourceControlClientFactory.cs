using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.Entity.CodeGenerator.Services.SourceControl
{
	public interface ISourceControlClientFactory
	{
		ISourceControlClient Create(string path);

		void Release(ISourceControlClient sourceControl);
	}
}
