using Havit.Web.Bootstrap.UI.WebControls;
using Havit.Web.UI.WebControls.ControlsValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace Havit.Web.Bootstrap.UI.WebControls.ControlsValues
{
	/// <summary>
	/// IPersisterControlExtender pro CollapsiblePanel.
	/// </summary>
	public class CollapsiblePanelPersisterControlExtender : IPersisterControlExtender
	{
		/// <summary>
		/// Získá hodnotu (stav) zadaného controlu.		
		/// </summary>
		public object GetValue(System.Web.UI.Control control)
		{
			CollapsiblePanel collapsiblePanel = (CollapsiblePanel)control;
			return collapsiblePanel.Collapsed;
		}

		/// <summary>
		/// Získá typ hodnoty zadaného controlu.
		/// </summary>		
		public Type GetValueType()
		{
			return typeof(bool);
		}

		/// <summary>
		/// Nastaví hodnotu do controlu.
		/// </summary>
		public void SetValue(System.Web.UI.Control control, object value)
		{
			CollapsiblePanel collapsiblePanel = (CollapsiblePanel)control;
			collapsiblePanel.Collapsed = (bool)value;
		}

		/// <summary>
		/// Vrací prioritu se kterou je tento IPersisterControlExtender použitelný
		/// pro zpracování daného controlu. 
		/// Slouží k řešení situací, kdy potřebujeme předka a potomka zpracovávat jinak
		/// (např. DropDownList a EnterpriseDropDownList - registrací na typ DDL
		/// se automaticky chytneme i na EDDL, proto v extenderu pro EDDL použijeme
		/// vyšší prioritu).
		/// Vyšší priorita vyhravá.Není-li control extender použitelný, nechť vrací null.
		/// </summary>
		public int? GetPriority(Control control)
		{
			if (control is CollapsiblePanel)
			{
				return 1;
			}
			return null;
		}

		/// <summary>
		/// Pokud je true, ControlsValuesPersister rekursivně projde i child controly.
		/// Implicitně vrací false.
		/// </summary>
		public bool PersistsChildren(Control control)
		{
			return true;
		}
	}
}
