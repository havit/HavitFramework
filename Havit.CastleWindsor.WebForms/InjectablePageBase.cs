using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI;

namespace Havit.CastleWindsor.WebForms
{
	public class InjectablePageBase : Page
	{
		public InjectablePageBase()
		{
			DependencyInjectionWebFormsHelper.InitializePage(this);
		}
	}
}
