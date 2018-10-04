using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace Havit.Web.UI.WebControls.ControlsValues
{
	/// <summary>
	/// Extender slouží k zpracování hodnoty controlu. Tedy pro získání hodnoty, 
	/// která má být uložena a pro nastavení uložené hodnoty.
	/// </summary>
	public interface IPersisterControlExtender
	{
		/// <summary>
		/// Získá hodnotu (stav) zadaného controlu.		
		/// </summary>
		object GetValue(Control control);

		/// <summary>
		/// Získá typ hodnoty zadaného controlu.
		/// </summary>		
		Type GetValueType();

		/// <summary>
		/// Nastaví hodnotu do controlu.
		/// </summary>
		void SetValue(Control control, object value);

		/// <summary>
		/// Vrací prioritu se kterou je tento IPersisterControlExtender použitelný
		/// pro zpracování daného controlu. 
		/// Slouží k řešení situací, kdy potřebujeme předka a potomka zpracovávat jinak
		/// (např. DropDownList a EnterpriseDropDownList - registrací na typ DDL
		/// se automaticky chytneme i na EDDL, proto v extenderu pro EDDL použijeme
		/// vyšší prioritu).
		/// Vyšší priorita vyhravá.Není-li control extender použitelný, nechť vrací null.
		/// </summary>
		int? GetPriority(Control control);

		/// <summary>
		/// Pokud je true, ControlsValuesPersister se pokusí uložit i hodnoty child controlů.
		/// Implicitně vrací false.
		/// </summary>		
		bool PersistsChildren(Control control);
	}
}
