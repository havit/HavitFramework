using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Extension metody k ListItem.
	/// </summary>
	public static class ListItemExtensions
	{
		/// <summary>
		/// Název atributu, ve kterém je uložena hodnota OptionGroup.
		/// </summary>
		internal const string OptionGroupAttributeName = "OptionGroup";

		/// <summary>
		/// Nastavuje hodnotu OptionGroup.
		/// </summary>
		public static void SetOptionGroup(this ListItem listItem, string value)
		{
			listItem.Attributes[OptionGroupAttributeName] = value;
		}

		/// <summary>
		/// Vrací hodnotu OptionGroup.
		/// </summary>
		public static string GetOptionGroup(this ListItem listItem)
		{
			return listItem.Attributes[OptionGroupAttributeName];
		}

		/// <summary>
		/// "Konvertuje" kolekci ListItemCollection jako IEnumerable&lt;ListItem&gt;.
		/// </summary>
		public static IEnumerable<ListItem> AsEnumerable(this ListItemCollection listItems)
		{
			return listItems.Cast<ListItem>();
		}
	}
}