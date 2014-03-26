using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Havit.Web.Bootstrap.UI.WebControls
{
	internal static class TabModeExtensions
	{
		internal static string GetCssClass(this TabMode tabMode)
		{
			switch (tabMode)
			{
				case TabMode.Tabs:
					return "nav nav-tabs";

				case TabMode.Pills:
					return "nav nav-pills";

				case TabMode.PillsStacked:
					return "nav nav-pills nav-stacked";
				
				default:
					throw new HttpException("Unknown TabMode value.");
			}
		}
	}
}
