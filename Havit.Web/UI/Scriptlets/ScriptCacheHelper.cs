using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Havit.Web.UI.Scriptlets
{
	/// <summary>
	/// Cache skriptù. Umožòuje sdílet skripty mezi instancemi skriptletu. Napø. pro skriptlet v øádcích repeateru, apod.
	/// </summary>
	internal static class ScriptCacheHelper
	{
		#region FunctionCache (private)
		/// <summary>
		/// Cache pro klientské skripty. Klíèem je skript a parametry funkce, hodnotou je název funkce,
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
					// pokud cache ještì není, vytvoøíme ji a vrátíme
					// žádné zámky (lock { ... }) nejsou potøeba, jsme stále v jednom HttpContextu
					result = new Dictionary<string, string>();
					HttpContext.Current.Items[typeof(ScriptCacheHelper)] = result;
				}

				return result;
			}
		}
		#endregion

		#region AddFunctionToCache
		/// <summary>
		/// Pøidá kód s parametry (klíè) do cache pod zadaný název funkce (hodnota).
		/// </summary>
		/// <param name="functionName">Název funkce, ve které je skript registrován.</param>
		/// <param name="functionParameters">Názvy parametrù funkce.</param>
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
		/// <param name="functionParameters">Názvy parametrù funkce.</param>
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
		/// Vrátí klíè do cache z parametrù.
		/// </summary>
		private static string GetCacheKey(string[] parameters, string code)
		{
			return String.Join("|", parameters) + "|" + code;
		}
		#endregion
	}
}
