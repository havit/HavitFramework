using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using Havit.Web.UI.WebControls.ControlsValues;

namespace Havit.Web.UI.WebControls.ControlsValues
{
	/// <summary>
	/// IPersisterControlExtender pro DateTimeBox.	
	/// </summary>
	public class DateTimeBoxPersisterControlExtender : IPersisterControlExtender
	{
		/// <summary>
		/// Získá hodnotu (stav) zadaného controlu.		
		/// </summary>
		public object GetValue(Control control)
		{
			DateTimeBox dateTimeBox = ((DateTimeBox)control);
			if (!dateTimeBox.IsValid)
			{
				return null;
			}
			return dateTimeBox.Value;
		}

		/// <summary>
		/// Získá typ hodnoty zadaného controlu.
		/// </summary>		
		public Type GetValueType()
		{
			return typeof(DateTime?);
		}

		/// <summary>
		/// Nastaví hodnotu do controlu.
		/// </summary>
		public void SetValue(Control control, object value)
		{
			DateTimeBox dateTimeBox = ((DateTimeBox)control);
			DateTime? dateTimeValue = (DateTime?)value;
			dateTimeBox.Value = dateTimeValue;
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
			if (control is DateTimeBox)
			{
				return 1;
			}
			return null;
		}

		/// <summary>
		/// Pokud je true, ControlsValuesPersister se pokusí uložit i hodnoty child controlů.
		/// Implicitně vrací false.
		/// </summary>
		public bool PersistsChildren(Control control)
		{
			return false;
		}
	}
}
