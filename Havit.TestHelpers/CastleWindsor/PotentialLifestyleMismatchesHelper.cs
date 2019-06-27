using Castle.Core;
using Castle.MicroKernel;
using Castle.Windsor;
using Castle.Windsor.Diagnostics;
using Castle.Windsor.Diagnostics.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Text;

namespace Havit.TestHelpers.CastleWindsor
{
	public static class PotentialLifestyleMismatchesHelper
	{
		public static void AssertPotentialLifestyleMismatches(IWindsorContainer container)
		{
			AssertPotentialLifestyleMismatches(container, cm => true);
		}

		public static void AssertPotentialLifestyleMismatches(IWindsorContainer container, Predicate<ComponentModel> componentModelPredicate)
		{
			var diagnostic = new PotentialLifestyleMismatchesDiagnostic(container.Kernel);
			IHandler[][] handlersArray = diagnostic.Inspect().Where(ha => componentModelPredicate(ha.Last().ComponentModel)).ToArray();

			if (handlersArray.Any())
			{
				var builder = new StringBuilder();
				builder.AppendFormat("Potential lifestyle mismatches ({0})\r\n", handlersArray.Count());
				foreach (IHandler[] handlers in handlersArray)
				{
					builder.AppendLine(GetMismatchMessage(handlers));
				}
				Assert.Fail(builder.ToString());
			}
		}

		private static string GetMismatchMessage(IHandler[] handlers)
		{
			var message = new StringBuilder();
			var root = handlers.First();
			var last = handlers.Last();
			message.AppendFormat("Component '{0}' with lifestyle {1} ", GetNameDescription(root.ComponentModel), root.ComponentModel.GetLifestyleDescription());
			message.AppendFormat("depends on '{0}' with lifestyle {1}", GetNameDescription(last.ComponentModel), last.ComponentModel.GetLifestyleDescription());

			for (var i = 1; i < handlers.Length - 1; i++)
			{
				var via = handlers[i];
				message.AppendLine();
				message.AppendFormat("\tvia '{0}' with lifestyle {1}", GetNameDescription(via.ComponentModel), via.ComponentModel.GetLifestyleDescription());
			}

			message.AppendLine();
			return message.ToString();
		}

		private static string GetName(IHandler[] handlers, IHandler root)
		{
			var indirect = handlers.Length > 2 ? "indirectly " : string.Empty;
			return string.Format("\"{0}\" »{1}« {2}depends on", GetNameDescription(root.ComponentModel), root.ComponentModel.GetLifestyleDescription(),
								 indirect);
		}

		private static string GetNameDescription(ComponentModel componentModel)
		{
			if (componentModel.ComponentName.SetByUser)
			{
				return componentModel.ComponentName.Name;
			}
			return componentModel.ToString();
		}
	}
}
