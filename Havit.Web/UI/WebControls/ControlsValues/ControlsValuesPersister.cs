using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Linq;

namespace Havit.Web.UI.WebControls.ControlsValues
{
	/// <summary>
	/// Control, který umí získat a nastavit stav vnořených controlů.
	/// Stavem se myslí zejména hodnoty zpracovávané uživateli v UI.
	/// Základními metodami, kterými se pracuje, jsou metody ApplyValues a RetrieveValues.
	/// </summary>
	public class ControlsValuesPersister: Control
	{
		#region PersisterExtenders
		/// <summary>
		/// Vrací úložiště extenderů.
		/// Extendery získávají/nastavují hodnoty controlům.
		/// </summary>
		// Připraveno na možnost konfigurace per instance, ala Scriptlet.
		// Nyní vrací jen výchozí hodnotu definovanou v PersisterControExtenderRepository. Proto private.
		private PersisterControlExtenderRepository PersisterExtenders
		{
			get
			{
				return PersisterControlExtenderRepository.Default;
			}
		} 
		#endregion

		#region ControlValueSet
		/// <summary>
		/// Událost oznamuje každé nastavení hodnoty do controlu.
		/// Je vyvolána poté, co PersisterControlExtender nastaví do controlu hodnotu.
		/// </summary>
		public event ControlValueSetEventHandler ControlValueSet; 
		#endregion

		#region ApplyValues
		/// <summary>
		/// Nastaví stav controlů dle předaných hodnot.
		/// </summary>
		public void ApplyValues(ControlsValuesHolder dataHolder)
		{
			if (this.HasControls())
			{
				foreach (Control nestedControl in this.Controls)
				{
					ApplyValues(dataHolder, nestedControl);
				}
			}
		}

		/// <summary>
		/// Nastaví stav controlů dle předaných hodnot.
		/// Controly se zpracovávají rekurzivně. Rekurze je zastavena nastavením 
		/// hodnoty do nějakého controlu (tj. podmínkou existence hodnoty
		/// a vhodného IPersisterControlExtenderu).
		/// </summary>
		/// <param name="dataHolder">Hodnoty ke zpracování.</param>
		/// <param name="control">Nastavovaný control.</param>
		private void ApplyValues(ControlsValuesHolder dataHolder, Control control)
		{
			string key = GetControlKey(control);
			if (dataHolder.ContainsKey(key))
			{
				IPersisterControlExtender persisterExtender = PersisterExtenders.FindExtender(control);
				if (persisterExtender != null)
				{
					object value = dataHolder.GetValue(key);
					// nastavíme do controlu hodnotu a oznámíme nastavení (událost)
					persisterExtender.SetValue(control, value);
					OnControlValueSet(new ControlValueEventArgs(control, value));
					// přerušíme rekurzi
					return;
				}
			}

			// nenašli jsme hodnotu a extender, tak pokračujeme rekurzivně ve stromu controlu
			if (this.HasControls())
			{
				foreach (Control nestedCotrol in control.Controls)
				{
					ApplyValues(dataHolder, nestedCotrol);
				}
			}
		} 
		#endregion

		#region OnControlValueSet
		/// <summary>
		/// Oznamuje nastavení hodnoty do controlu.
		/// </summary>
		protected virtual void OnControlValueSet(ControlValueEventArgs eventArgs)
		{
			if (this.ControlValueSet != null)
			{
				this.ControlValueSet(this, eventArgs);
			}
		} 
		#endregion

		#region RetrieveValues
		/// <summary>
		/// Získá stav controlů a vrátí jej.		
		/// </summary>
		public ControlsValuesHolder RetrieveValues()
		{
			ControlsValuesHolder result = new ControlsValuesHolder();
			RetrieveValues(result);
			return result;
		}

		/// <summary>
		/// Uloží stav controlů do předaného úložiště.
		/// </summary>
		public void RetrieveValues(ControlsValuesHolder dataHolder)
		{
			if (this.HasControls())
			{
				foreach (Control nestedControl in this.Controls)
				{
					RetrieveValues(dataHolder, nestedControl);
				}
			}
		} 

		/// <summary>
		/// Uloží stav controlu do předaného úložiště.
		/// Není-li pro control nalezen IPersisterControlExtender, ukládá stav rekurzivně do hloubky.
		/// </summary>
		protected void RetrieveValues(ControlsValuesHolder dataHolder, Control control)
		{
			IPersisterControlExtender persisterExtender = PersisterExtenders.FindExtender(control);
			if (persisterExtender != null)
			{
				string key = GetControlKey(control);
				object value = persisterExtender.GetValue(control);
				dataHolder.SetValue(key, value);
			}
			else
			{
				if (this.HasControls())
				{
					foreach (Control nestedCotrol in control.Controls)
					{
						RetrieveValues(dataHolder, nestedCotrol);
					}
				}
			}
		}
		#endregion

		#region GetControlKey, GetControlID
		/// <summary>
		/// Vrátí klíč použitý pro uložení hodnoty pro zadaný control.
		/// Klíč se získává složením ID controlu a nadřazených naming containerů od ControlsValuesPersisteru dolu,
		/// oddělovačem je lomítko.
		/// Nemá-li control ID, použije se automaticky doplněné ("ctl00"), apod.
		/// </summary>
		private string GetControlKey(Control control)
		{
			string result = GetControlID(control);
			control = control.Parent;
			while (control != this)
			{
				if (control is INamingContainer)
				{
					// rekurze zdola nahoru, proto doplňujeme na začátek
					result = GetControlID(control) + "/" + result;
				}
				control = control.Parent;
			}
			return result;
		}

		/// <summary>
		/// Vrací ID controlu, není-li zadáno, vrací automaticky vygenerované to "ctl00", apod.
		/// Pomocná metoda pro metodu GetControlKey.
		/// </summary>
		private string GetControlID(Control control)
		{
			string result = control.ID;
			if (result == null)
			{
				int lastIndex = control.UniqueID.LastIndexOf("$");
				if (lastIndex >= 0)
				{
					result = control.UniqueID.Substring(lastIndex + 1);
				}
				else
				{
					result = control.UniqueID;
				}
			}
			return result;
		} 
		#endregion
	}
}
