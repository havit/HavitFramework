using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Havit.Web.UI.Scriptlets
{
	/// <summary>
	/// Cache skriptů. Umožňuje sdílet skripty mezi instancemi skriptletu. Např. pro skriptlet v řádcích repeateru, apod.
	/// </summary>
	internal static class ScriptCacheHelper
	{
		#region FunctionCache (private)
		/// <summary>
		/// Cache pro klientské skripty. Klíčem je skript a parametry funkce, hodnotou je název funkce,
		/// ve které je skript registrován.
		/// Cache je uložena v HttpContextu.
		/// </summary>
		private static Dictionary<string, string> FunctionCache
		{
			get
			{
				Dictionary<string, string> result = (Dictionary<string, string>)HttpContext.Current.Items[typeof(ScriptCacheHelper)];

				if (result == null)
				{
					// pokud cache ještě není, vytvoříme ji a vrátíme
					// žádné zámky (lock { ... }) nejsou potřeba, jsme stále v jednom HttpContextu
					result = new Dictionary<string, string>();
					HttpContext.Current.Items[typeof(ScriptCacheHelper)] = result;
				}

				return result;
			}
		}
		#endregion

		#region AddFunctionToCache
		/// <summary>
		/// Přidá kód s parametry (klíč) do cache pod zadaný název funkce (hodnota).
		/// </summary>
		/// <param name="functionName">Název funkce, ve které je skript registrován.</param>
		/// <param name="functionParameters">Názvy parametrů funkce.</param>
		/// <param name="functionCode">Kód funkce.</param>
		public static void AddFunctionToCache(string functionName, string[] functionParameters, string functionCode)
		{
			FunctionCache.Add(GetCacheKey(functionParameters, functionCode), functionName);
		}		
		#endregion

		#region GetFunctionNameFromCache
		/// <summary>
		/// Vyhledá v cache a vrátí název funkce, se stejnými parametry a kódem skriptu.
		/// Pokud není název funkce nalezen, vrací null.
		/// </summary>
		/// <param name="functionParameters">Názvy parametrů funkce.</param>
		/// <param name="functionCode">Kód funkce.</param>
		public static string GetFunctionNameFromCache(string[] functionParameters, string functionCode)
		{
			string result;
			if (FunctionCache.TryGetValue(GetCacheKey(functionParameters, functionCode), out result))
			{
				return result;
			}
			else
			{
				return null;
			}
		}
		#endregion

		#region GetCacheKey (private)
		/// <summary>
		/// Vrátí klíč do cache z parametrů.
		/// </summary>
		private static string GetCacheKey(string[] parameters, string code)
		{
			return String.Join("|", parameters) + "|" + code;
		}
		#endregion
	}
}
