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
	public class ControlsValuesPersister : Control
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
			ApplyValues(dataHolder, this, this, this.PersisterExtenders, (args) => this.OnControlValueSet(args));
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
			RetrieveValues(dataHolder, this, this, PersisterExtenders);
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

		#region RetrieveValues (internal static)
		/// <summary>
		/// Uloží stav controlu do předaného úložiště.
		/// Není-li pro control nalezen IPersisterControlExtender, ukládá stav rekurzivně do hloubky.
		/// </summary>
		internal static ControlsValuesHolder RetrieveValues(Control control)
		{
			ControlsValuesHolder dataHolder = new ControlsValuesHolder();
			RetrieveValues(dataHolder, control, control, PersisterControlExtenderRepository.Default);
			return dataHolder;
		}

		private static void RetrieveValues(ControlsValuesHolder dataHolder, Control control, Control containerControl, PersisterControlExtenderRepository persisterControlExtenderRepository)
		{			
			IPersisterControlExtender persisterExtender = persisterControlExtenderRepository.FindExtender(control);		
			if (persisterExtender != null)
			{
				string key = GetControlKey(control, containerControl);
				object value = persisterExtender.GetValue(control);				
				dataHolder.SetValue(key, value);
				if (!persisterExtender.PersistChildren(control))
				{
					return;
				}
			}
					
			if (control.HasControls())
			{
				foreach (Control nestedCotrol in control.Controls)
				{
					RetrieveValues(dataHolder, nestedCotrol, containerControl, persisterControlExtenderRepository);
				}
			}			
		}
		#endregion

		#region ApplyValues (internal static)
		/// <summary>
		/// Nastaví stav controlů dle předaných hodnot.
		/// Controly se zpracovávají rekurzivně. Rekurze je zastavena nastavením 
		/// hodnoty do nějakého controlu (tj. podmínkou existence hodnoty
		/// a vhodného IPersisterControlExtenderu).
		/// </summary>
		/// <param name="dataHolder">Hodnoty ke zpracování.</param>
		/// <param name="control">Nastavovaný control.</param>
		internal static void ApplyValues(ControlsValuesHolder dataHolder, Control control)
		{
			ApplyValues(dataHolder, control, control, PersisterControlExtenderRepository.Default, null);
		}

		private static void ApplyValues(ControlsValuesHolder dataHolder, Control control, Control containerControl, PersisterControlExtenderRepository persisterControlExtenderRepository, Action<ControlValueEventArgs> callback = null)
		{
			IPersisterControlExtender persisterExtender = persisterControlExtenderRepository.FindExtender(control);			
			if (persisterExtender != null)
			{
				string key = GetControlKey(control, containerControl);
				if (dataHolder.ContainsKey(key))
				{
					object value = dataHolder.GetValue(key);
					// nastavíme do controlu hodnotu a oznámíme nastavení (callback)
					persisterExtender.SetValue(control, value);
					if (callback != null)
					{
						callback(new ControlValueEventArgs(control, value));
					}
				}

				if (!persisterExtender.PersistChildren(control))
				{
					return;
				}
			}

			// nenašli jsme extender, tak pokračujeme rekurzivně ve stromu controlu
			if (control.HasControls())
			{
				foreach (Control nestedCotrol in control.Controls)
				{
					ApplyValues(dataHolder, nestedCotrol, containerControl, persisterControlExtenderRepository);
				}
			}
		}
		#endregion

		#region GetControlKey, GetControlID (static)
		/// <summary>
		/// Vrátí klíč použitý pro uložení hodnoty pro zadaný control.
		/// Klíč se získává složením ID controlu a nadřazených naming containerů od ControlsValuesPersisteru dolu,
		/// oddělovačem je lomítko.
		/// Nemá-li control ID, použije se automaticky doplněné ("ctl00"), apod.
		/// </summary>
		private static string GetControlKey(Control control, Control containerControl)
		{
			if (control == containerControl)
			{
				return "!";
			}

			string result = GetControlID(control);
			control = control.Parent;
			while (control != containerControl)
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
		private static string GetControlID(Control control)
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
