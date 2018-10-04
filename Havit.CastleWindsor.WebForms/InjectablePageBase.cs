using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI;

namespace Havit.CastleWindsor.WebForms
{
	/// <summary>
	/// Base class for pages with dependency injection where it is not possible to use DependencyInjectionPageHandlerFactory.
	/// </summary>
	public class InjectablePageBase : Page
	{
		/// <summary>
		/// Constructor.
		/// Resolves dependencies.
		/// </summary>
		public InjectablePageBase()
		{
			DependencyInjectionWebFormsHelper.InitializePage(this);
		}
	}
}
