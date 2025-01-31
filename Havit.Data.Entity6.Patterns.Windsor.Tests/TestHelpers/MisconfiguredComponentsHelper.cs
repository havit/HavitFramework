using Castle.MicroKernel;
using Castle.MicroKernel.Handlers;
using Castle.Windsor;
using Castle.Windsor.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Text;

namespace Havit.Data.Entity6.Patterns.Windsor.Tests.TestHelpers;


public static class MisconfiguredComponentsHelper
{
	public static void AssertMisconfiguredComponents(IWindsorContainer container)
	{
		var diagnostic = new PotentiallyMisconfiguredComponentsDiagnostic(container.Kernel);
		IHandler[] handlers = diagnostic.Inspect();
		if (handlers != null && handlers.Any())
		{
			var builder = new StringBuilder();
			builder.AppendFormat("Misconfigured components ({0})\r\n", handlers.Count());
			foreach (IHandler handler in handlers)
			{
				var info = (IExposeDependencyInfo)handler;
				var inspector = new DependencyInspector(builder);
				info.ObtainDependencyDetails(inspector);
			}
			Assert.Fail(builder.ToString());
		}
	}
}